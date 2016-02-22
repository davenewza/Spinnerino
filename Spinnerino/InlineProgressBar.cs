﻿using System;
using System.Threading;
using Timer = System.Timers.Timer;

namespace Spinnerino
{
    public class InlineProgressBar : IDisposable, IProgressPercentageIndicator
    {
        readonly int _width;
        readonly char _completedChar;
        readonly char _notCompletedChar;
        readonly Timer _timer = new Timer(65);
        readonly int _cursorLeft;

        double _progress;

        public InlineProgressBar(int width = 10, char completedChar = '#', char notCompletedChar = '-')
        {
            _width = width;
            _completedChar = completedChar;
            _notCompletedChar = notCompletedChar;
            _cursorLeft = Console.CursorLeft;

            _timer.Elapsed += (o, ea) =>
            {
                Print(_progress);
            };

            _timer.Start();
        }

        void Print(double progress)
        {
            Console.CursorLeft = _cursorLeft;

            var lineLength = _width;
            var progressLength = (int)(progress / 100.0 * lineLength);
            var theRestLength = lineLength - progressLength;
            var progressLine = new string(_completedChar, progressLength);
            var theRestLine = new string(_notCompletedChar, theRestLength);

            Console.Write($"[{progressLine}{theRestLine}] {progress:0.##} %     ");
        }

        public void SetProgress(double percentage)
        {
            Interlocked.Exchange(ref _progress, Math.Min(100, Math.Max(percentage, 0)));
        }

        public void Dispose()
        {
            _timer.Dispose();

            Print(100);
            Console.WriteLine();
        }
    }
}