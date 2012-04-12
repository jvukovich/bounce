using System.Collections.Generic;
using System.Messaging;

namespace Bounce.Framework.Obsolete {
    public class MsmqQueue : Task {
        [Dependency]
        public Task<bool> Transactional;
        [Dependency]
        public Task<bool> Private;
        [Dependency]
        public Task<string> Name;
        [Dependency]
        public Task<string> Machine;
        [Dependency]
        public Task<IEnumerable<MsmqUserPermissions>> Permissions;

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