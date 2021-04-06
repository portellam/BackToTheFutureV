﻿using FusionLibrary;
using FusionLibrary.Extensions;
using GTA;
using System;
using System.Collections.Generic;
using System.IO;
using static BackToTheFutureV.InternalEnums;

namespace BackToTheFutureV
{
    [Serializable]
    internal class WaybackMachine
    {
        public List<WaybackPed> Replicas { get; } = new List<WaybackPed>();

        public Guid GUID { get; private set; } = Guid.Empty;

        public PedReplica PedReplica { get; private set; }

        private int PedHandle { get; set; }

        public Ped Ped
        {
            get => (Ped)Entity.FromHandle(PedHandle);
            private set
            {
                if (value.NotNullAndExists())
                    PedHandle = value.Handle;
                else
                    PedHandle = 0;
            }
        }

        public int LastRecReplicaIndex { get; private set; } = 0;
        public WaybackPed LastRecReplica => Replicas[LastRecReplicaIndex];

        public int ReplicaIndex { get; private set; } = 0;
        public WaybackPed CurrentReplica => Replicas[ReplicaIndex];
        public WaybackPed PreviousReplica
        {
            get
            {
                if (ReplicaIndex == 0)
                    return CurrentReplica;

                return Replicas[ReplicaIndex - 1];
            }
        }
        public WaybackPed NextReplica
        {
            get
            {
                if (ReplicaIndex == Replicas.Count - 1)
                    return CurrentReplica;

                return Replicas[ReplicaIndex + 1];
            }
        }

        public float AdjustedRatio => Game.LastFrameTime;
        //{
        //    get
        //    {
        //        if (CurrentReplica.Timestamp == NextReplica.Timestamp)
        //            return 0;

        //        return (Game.GameTime - StartPlayGameTime - CurrentReplica.Timestamp) / (NextReplica.Timestamp - CurrentReplica.Timestamp);
        //    }
        //}

        public float StartRecGameTime { get; private set; }
        public float StartPlayGameTime { get; private set; }

        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }

        public WaybackStatus Status { get; private set; } = WaybackStatus.Idle;

        public bool WaitForReentry { get; set; } = false;

        public WaybackMachine(Ped ped, Guid guid)
        {
            Ped = ped;
            GUID = guid;

            PedReplica = new PedReplica(Ped);

            StartRecGameTime = Game.GameTime;

            Replicas.Add(new WaybackPed(GUID, Ped, StartRecGameTime));

            StartTime = LastRecReplica.Time;

            Status = WaybackStatus.Recording;
        }

        public void StartOn(Ped ped, bool waitForReentry = false)
        {
            Ped = ped;
            WaitForReentry = waitForReentry;

            StartTime = Utils.CurrentTime;
        }

        public void Tick()
        {
            switch (Status)
            {
                case WaybackStatus.Idle:
                    if (Utils.CurrentTime.Between(StartTime, EndTime))
                    {
                        if (WaitForReentry)
                        {
                            if (TimeMachineHandler.GetTimeMachineFromReplicaGUID(GUID).Properties.TimeTravelPhase == TimeTravelPhase.Reentering)
                                return;

                            ReplicaIndex = 0;
                            WaitForReentry = false;
                        }
                        else
                            ReplicaIndex = Replicas.FindIndex(x => x.Time >= Utils.CurrentTime);

                        StartPlayGameTime = Game.GameTime + CurrentReplica.Timestamp;
                        Status = WaybackStatus.Playing;

                        if (!Ped.NotNullAndExists())
                            Spawn();

                        Play();
                    }
                    break;
                case WaybackStatus.Recording:
                    Record();
                    break;
                case WaybackStatus.Playing:
                    Play();
                    break;
            }
        }

        private void Spawn()
        {
            Ped = World.GetClosestPed(Utils.Lerp(CurrentReplica.Position, NextReplica.Position, AdjustedRatio), 1, PedReplica.Model);

            if (Ped.NotNullAndExists())
                return;

            Ped = PedReplica.Spawn(Utils.Lerp(CurrentReplica.Position, NextReplica.Position, AdjustedRatio), Utils.Lerp(CurrentReplica.Heading, NextReplica.Heading, AdjustedRatio));
        }

        public WaybackPed Record(WaybackVehicle waybackVehicle = default)
        {
            if (Status != WaybackStatus.Recording)
                return null;

            if (!Ped.NotNullAndExists() || Utils.CurrentTime < StartTime)
            {
                Stop();
                return null;
            }

            WaybackPed recordedReplica = new WaybackPed(GUID, Ped, StartRecGameTime);

            if (recordedReplica.Timestamp == LastRecReplica.Timestamp)
            {
                if (waybackVehicle != default)
                    LastRecReplica.WaybackVehicle = waybackVehicle;

                return null;
            }

            if (waybackVehicle != default)
                recordedReplica.WaybackVehicle = waybackVehicle;

            Replicas.Add(recordedReplica);

            LastRecReplicaIndex++;

            return recordedReplica;
        }

        private void Play()
        {
            if (!Ped.NotNullAndExists())
                return;

            CurrentReplica.Apply(Ped, NextReplica, AdjustedRatio);

            if (ReplicaIndex == Replicas.Count - 1)
                Status = WaybackStatus.Idle;
            else
                ReplicaIndex++;
        }

        public void Stop()
        {
            if (Status == WaybackStatus.Recording)
                EndTime = LastRecReplica.Time.AddMinutes(-1);

            ReplicaIndex = 0;
            PedHandle = 0;
            Status = WaybackStatus.Idle;
        }

        public static WaybackMachine FromData(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                try
                {
                    return (WaybackMachine)Utils.BinaryFormatter.Deserialize(stream);
                }
                catch
                {
                    return null;
                }
            }
        }

        public static implicit operator byte[](WaybackMachine command)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Utils.BinaryFormatter.Serialize(stream, command);
                return stream.ToArray();
            }
        }
    }
}
