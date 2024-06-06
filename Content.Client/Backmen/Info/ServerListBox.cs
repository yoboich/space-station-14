using Robust.Client;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Utility;

namespace Content.Client.Info
{
    public sealed class ServerListBox : BoxContainer
    {
        private IGameController _gameController;
        private List<Button> _connectButtons = new();

        public ServerListBox()
        {
            _gameController = IoCManager.Resolve<IGameController>();
            Orientation = LayoutOrientation.Vertical;
            AddServers();
        }

        private void AddServers()
        {
            AddServerInfo("ФОБОС", "MRP Проект для ценителей драйва");
            AddServerInfo("ТИТАН", "HRP Проект для ценителей RP");
            AddServerInfo("ДЭЙМОС", "MRP Проект для ценителей спокойствия");
            AddServerInfo("СОЮЗ - 1", "MRP Проект в сеттинге СССП");
            AddServerInfo("ФРОНТИР", "MRP Проект в сеттинге Eve Online");
            AddServerInfo("МОРПЕХИ", "MRP Проект в сеттинге Aliens");
            AddServerInfo("XXX", "XXX");
        }

        private void AddServerInfo(string serverName, string description)
        {
            var serverBox = new BoxContainer
            {
                Orientation = LayoutOrientation.Horizontal,
            };

            var nameAndDescriptionBox = new BoxContainer
            {
                Orientation = LayoutOrientation.Vertical,
            };

            var serverNameLabel = new Label
            {
                Text = serverName,
                MinWidth = 200
            };

            var descriptionLabel = new RichTextLabel
            {
                MaxWidth = 500
            };
            descriptionLabel.SetMessage(FormattedMessage.FromMarkup(description));

            var buttonBox = new BoxContainer
            {
                Orientation = LayoutOrientation.Vertical,
                HorizontalExpand = true,
                HorizontalAlignment = HAlignment.Right
            };

            var connectButton = new Button
            {
                Text = "Connect"
            };

            _connectButtons.Add(connectButton);

            connectButton.OnPressed += _ =>
            {
                ConnectToServer(serverName);

                foreach (var connectButton in _connectButtons)
                {
                    connectButton.Disabled = true;
                }
            };

            buttonBox.AddChild(connectButton);

            nameAndDescriptionBox.AddChild(serverNameLabel);
            nameAndDescriptionBox.AddChild(descriptionLabel);

            serverBox.AddChild(nameAndDescriptionBox);
            serverBox.AddChild(buttonBox);

            AddChild(serverBox);
        }

        private void ConnectToServer(string serverName)
        {
            var url = "";

            switch (serverName)
            {
                case "ДЭЙМОС":
                    url = "ss14://f3.deadspace14.net:1213";
                    break;
                case "МОРПЕХИ":
                    url = "ss14://marines.deadspace14.net:1214";
                    break;
                case "СОЮЗ - 1":
                    url = "ss14://s1.deadspace14.net:1215";
                    break;
                case "ТИТАН":
                    url = "ss14://f2.deadspace14.net:1212";
                    break;
                case "ФОБОС":
                    url = "ss14://f1.deadspace14.net:1212";
                    break;
                case "ФРОНТИР":
                    url = "ss14://ff.deadspace14.net:1213";
                    break;
                case "XXX":
                    url = "ss14://localhost:1212";
                    break;
            }
            _gameController.Redial(url, "Connecting to another server...");
        }
    }
}
