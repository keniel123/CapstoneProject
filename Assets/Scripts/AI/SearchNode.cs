﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


class SearchNode : MonoBehaviour
    {
        public SearchNode Parent;
        public bool InOpenList;
        public bool InClosedList;
        public float DistanceToGoal;
        public float DistanceTraveled;

        public Point Position;
        public bool Walkable;
        public SearchNode[] Neighbors;
    }

