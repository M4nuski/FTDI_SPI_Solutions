using System;
using System.Threading;

namespace ScriptLoopTest
{
    class ThreadBlockingClass
    {
        public EventHandler OnUpdate;

        private int Delay = 100;

        public string msg;

        public ThreadBlockingClass(int delay)
        {
            Delay = delay;
        }

        public void update()
        {
            Thread.Sleep(Delay);
            SafeCallEvent();
        }

        public void update(int count)
        {
            for (var i = 0; i < count; i++)
            {
                update();

            }
        }

        private void SafeCallEvent()
        {
            if (OnUpdate != null) OnUpdate(this, null);
        }

        public void update20()
        {
            for (var i = 0; i < 20; i++)
            {
                update();
            }
        }
    }
}
