using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiplayerUNO.UI.Animations {
    public class AnimationController {
        public BlockingCollection<Animation> AnimationQueue;

        public AnimationController() {
            AnimationQueue = new BlockingCollection<Animation>();
        }

        public void Start() {
            new Thread(() => {
                WaitAndRun();
            }).Start();
        }

        public void WaitAndRun() {
            while (true) {
                //var anima = AnimationQueue.Take();
                //if (anima()) {
                //    AnimationQueue.Add(anima);
                //}
            }
        }

        public void AddAnimation(Animation anima) {
            AnimationQueue.Add(anima);
        }
    }
}