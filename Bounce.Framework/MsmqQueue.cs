using System.Messaging;

namespace Bounce.Framework {
    public class MsmqQueue : Task {
        public Val<bool> Transactional;
        public Val<bool> Private;
        public Val<string> Name;
        public Val<string> Machine;

        public MsmqQueue() {
            Machine = ".";
            Transactional = false;
            Private = true;
        }

        public override void Build() {
            if (!MessageQueue.Exists(QueuePath)) {
                MessageQueue.Create(QueuePath, Transactional.Value);
            }
        }

        public override void Clean() {
            if (MessageQueue.Exists(QueuePath)) {
                MessageQueue.Delete(QueuePath);
            }
        }

        private string QueuePath {
            get {
                return string.Format(@"{0}\{1}{2}", Machine.Value, Private.Value ? @"Private$\" : "", Name.Value);
            }
        }
    }
}