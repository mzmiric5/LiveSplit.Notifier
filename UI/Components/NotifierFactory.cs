using LiveSplit.Model;
using LiveSplit.UI.Components;
using System;

[assembly: ComponentFactory(typeof(NotifierFactory))]

namespace LiveSplit.UI.Components
{
    public class NotifierFactory : IComponentFactory
    {
        public string ComponentName => "Event Notifier";

        public string Description => "Triggers webhooks or fires websocket events for different situations.";

        public ComponentCategory Category => ComponentCategory.Other;

        public IComponent Create(LiveSplitState state) => new NotifierComponent(state);

        public string UpdateName => ComponentName;

        public string XMLURL => "http://misozmiric.com/LiveSplit/myComponents/Notifier/update.LiveSplit.Notifier.xml";

        public string UpdateURL => "http://misozmiric.com/LiveSplit/myComponents/update/";

        public Version Version => Version.Parse("1.0.0");
    }
}
