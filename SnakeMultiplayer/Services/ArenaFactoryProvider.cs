// File: SnakeMultiplayer/Services/ArenaFactoryProvider.cs
using System;

namespace SnakeMultiplayer.Services
{
    public static class ArenaFactoryProvider
    {
        public static IArenaFactory GetFactory(int level)
        {
            return level switch
            {
                1 => new Level1ArenaFactory(),
                2 => new Level2ArenaFactory(),
                3 => new Level3ArenaFactory(),
                _ => throw new ArgumentException("Invalid level", nameof(level)),
            };
        }
    }
}