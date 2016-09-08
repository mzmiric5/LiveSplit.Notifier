using LiveSplit.Model;
using LiveSplit.Options;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public class NotifierComponent : LogicComponent, IDeactivatableComponent
    {
        public override string ComponentName => "Event Notifier";

        public bool Activated { get; set; }

        private LiveSplitState State { get; set; }
        private NotifierSettings Settings { get; set; }

        public ClientWebSocket WebSocket { get; set; }

        public NotifierComponent(LiveSplitState state)
        {
            Activated = true;

            State = state;
            Settings = new NotifierSettings(this);

            State.OnStart += State_OnStart;
            State.OnSplit += State_OnSplit;
            State.OnSkipSplit += State_OnSkipSplit;
            State.OnUndoSplit += State_OnUndoSplit;
            State.OnPause += State_OnPause;
            State.OnResume += State_OnResume;
            State.OnReset += State_OnReset;

            WebSocket = new ClientWebSocket();
        }

        public async Task Client(Uri uri, Action callback) {
            await WebSocket.ConnectAsync(uri, CancellationToken.None);
            callback();
            var buffer = new byte[1024];
            while (true) {
                var segment = new ArraySegment<byte>(buffer);
                var result = await WebSocket.ReceiveAsync(segment, CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close) {
                    await WebSocket.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "I don't do binary!", CancellationToken.None);
                    return;
                }
                int count = result.Count;
                while (!result.EndOfMessage) {
                    if (count >= buffer.Length) {
                        await WebSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Message too long", CancellationToken.None);
                        return;
                    }
                    segment = new ArraySegment<byte>(buffer, count, buffer.Length - count);
                    result = await WebSocket.ReceiveAsync(segment, CancellationToken.None);
                    count += result.Count;
                }

                var message = Encoding.UTF8.GetString(buffer, 0, count);
                Console.WriteLine('>' + message);
            }
        }

        public override void Dispose()
        {
            State.OnStart -= State_OnStart;
            State.OnSplit -= State_OnSplit;
            State.OnSkipSplit -= State_OnSkipSplit;
            State.OnUndoSplit -= State_OnUndoSplit;
            State.OnPause -= State_OnPause;
            State.OnResume -= State_OnResume;
            State.OnReset -= State_OnReset;

            WebSocket.Dispose();
        }

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode) { }

        public override Control GetSettingsControl(LayoutMode mode)
        {
            return Settings;
        }

        public override XmlNode GetSettings(XmlDocument document)
        {
            return Settings.GetSettings(document);
        }

        public override void SetSettings(XmlNode settings)
        {
            Settings.SetSettings(settings);
        }

        private void State_OnStart(object sender, EventArgs e)
        {
            SendPayload("Start");
        }

        private void State_OnSplit(object sender, EventArgs e)
        {
            if (State.CurrentPhase == TimerPhase.Ended)
            {
                if ((State.Run.Last().PersonalBestSplitTime[State.CurrentTimingMethod] == null || State.Run.Last().SplitTime[State.CurrentTimingMethod] < State.Run.Last().PersonalBestSplitTime[State.CurrentTimingMethod]) && Settings.PersonalBest)
                    SendPayload("PersonalBest");
                else {
                    if (Settings.NotAPersonalBest)
                        SendPayload("NotAPersonalBest");
                }
                    
            }
            else
            {
                string payload = null;

                var splitIndex = State.CurrentSplitIndex - 1;
                var timeDifference = State.Run[splitIndex].SplitTime[State.CurrentTimingMethod] - State.Run[splitIndex].Comparisons[State.CurrentComparison][State.CurrentTimingMethod];

                if (timeDifference != null) {
                    if (timeDifference < TimeSpan.Zero) {
                        payload = "SplitAheadGaining";

                        if (LiveSplitStateHelper.GetPreviousSegmentDelta(State, splitIndex, State.CurrentComparison, State.CurrentTimingMethod) > TimeSpan.Zero) {
                            payload = "SplitAheadLosing";
                        }
                    } else {
                        payload = "SplitBehindLosing";

                        if (LiveSplitStateHelper.GetPreviousSegmentDelta(State, splitIndex, State.CurrentComparison, State.CurrentTimingMethod) < TimeSpan.Zero) {
                            payload = "SplitBehindGaining";
                        }
                    }
                }

                //Check for best segment
                TimeSpan? curSegment = LiveSplitStateHelper.GetPreviousSegmentTime(State, splitIndex, State.CurrentTimingMethod);

                if (curSegment != null) {
                    if (State.Run[splitIndex].BestSegmentTime[State.CurrentTimingMethod] == null || curSegment < State.Run[splitIndex].BestSegmentTime[State.CurrentTimingMethod]) {
                        payload = "BestSegment";
                    }
                }

                if (string.IsNullOrEmpty(payload))
                    payload = "Split";

                SendPayload(payload);
            }
        }

        private void State_OnSkipSplit(object sender, EventArgs e)
        {
            if (Settings.SkipSplit) {
                SendPayload("Skip");
            }
        }

        private void State_OnUndoSplit(object sender, EventArgs e)
        {
            if (Settings.UndoSplit)
                SendPayload("Undo");
        }

        private void State_OnPause(object sender, EventArgs e)
        {
            if (Settings.Pause)
                SendPayload("Pause");
        }

        private void State_OnResume(object sender, EventArgs e)
        {
            if (Settings.Resume)
                SendPayload("Resume");
        }

        private void State_OnReset(object sender, TimerPhase e)
        {
            if (e != TimerPhase.Ended && Settings.Reset)
                SendPayload("Reset");
        }

        private void SendPayload(string eventType) {
            if (Activated) {
                if (Settings.isWH) {
                    Task.Factory.StartNew(() => {
                        try {
                            WebRequest request = WebRequest.Create(Settings.Url);
                            request.Method = "POST";
                            byte[] byteArray = Encoding.UTF8.GetBytes(eventType);
                            request.ContentType = "application/x-www-form-urlencoded";
                            request.ContentLength = byteArray.Length;
                            Stream dataStream = request.GetRequestStream();
                            dataStream.Write(byteArray, 0, byteArray.Length);
                            dataStream.Close();
                            WebResponse response = request.GetResponse();
                            var status = ((HttpWebResponse)response).StatusCode;
                            response.Close();
                        } catch (Exception e) {
                            Log.Error(e);
                        }
                    });
                } else if (Settings.isWS) {
                    if (WebSocket.State != WebSocketState.Open) {
                        ConnectToWS(() => {
                            sendWSData(eventType);
                        });
                    } else {
                        sendWSData(eventType);
                    }
                    
                }
            }
        }

        private void sendWSData(string eventType) {
            byte[] buffer = Encoding.UTF8.GetBytes(eventType);
            var segment = new ArraySegment<byte>(buffer);
            WebSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public void sendTest(string eventType) {
            SendPayload(eventType);
        }

        public void ConnectToWS(Action callback) {
            WebSocket = new ClientWebSocket();
            if (Settings.isWS && !string.IsNullOrEmpty(Settings.Url)) {
                var uri = new Uri(Settings.Url);
                var clientTask = Client(uri, callback);
            }
        }

        public void DisconnectFromWS() {
            if (WebSocket.State == WebSocketState.Open || WebSocket.State == WebSocketState.Connecting) {
                WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed connection", CancellationToken.None);
                WebSocket.Dispose();
            }
        }

        public int GetSettingsHashCode() => Settings.GetSettingsHashCode();
    }
}
