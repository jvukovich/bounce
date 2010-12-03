using System.Collections.Generic;
using System.Messaging;

namespace Bounce.Framework {
    public class MsmqQueue : Task {
        public Future<bool> Transactional;
        public Future<bool> Private;
        public Future<string> Name;
        public Future<string> Machine;
        public Future<IEnumerable<MsmqUserPermissions>> Permissions;

        public MsmqQueue() {
            Machine = ".";
            Transactional = false;
            Private = true;
            Permissions = new MsmqUserPermissions[0];
        }

        public override void Build() {
            if (!MessageQueue.Exists(QueuePath)) {
                var mq = MessageQueue.Create(QueuePath, Transactional.Value);

                foreach (var permission in Permissions.Value) {
                    mq.SetPermissions(permission.UserName, permission.Permissions);
                }
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

    public class MsmqUserPermissions {
        public string UserName;
        public MessageQueueAccessRights Permissions;
    }
}