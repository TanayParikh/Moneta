﻿namespace LottieUWP
{
    public abstract class PathIterator
    {
        public enum ContourType
        {
            Arc,
            MoveTo,
            Line,
            Close,
            Bezier
        }

        public abstract bool Next();

        public abstract bool Done { get; }

        public abstract ContourType CurrentSegment(float[] points);
    }
}