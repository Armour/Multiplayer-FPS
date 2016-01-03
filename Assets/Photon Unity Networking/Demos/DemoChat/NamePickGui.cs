using UnityEngine;


[RequireComponent(typeof(ChatGui))]
public class NamePickGui : MonoBehaviour
{
    public Vector2 GuiSize = new Vector2(200, 300);
    public string InputLine = string.Empty;

    private Rect guiCenteredRect;
    private ChatGui chatComponent;
    private string helpText = "Welcome to the Photon Chat demo.\nEnter a nickname to start. This demo does not require users to authenticate.";
    private const string UserNamePlayerPref = "NamePickUserName";

    public void Awake()
    {
        this.guiCenteredRect = new Rect(Screen.width/2-GuiSize.x/2, Screen.height/2-GuiSize.y/4, GuiSize.x, GuiSize.y);
        this.chatComponent = this.GetComponent<ChatGui>();

        if (this.chatComponent != null && chatComponent.enabled)
        {
            Debug.LogWarning("When using the NamePickGui, ChatGui should be disabled initially.");
            
            if (this.chatComponent.chatClient != null)
            {
                this.chatComponent.chatClient.Disconnect();
            }
            this.chatComponent.enabled = false;
        }

        string prefsName = PlayerPrefs.GetString(NamePickGui.UserNamePlayerPref);
        if (!string.IsNullOrEmpty(prefsName))
        {
            this.InputLine = prefsName;
        }
    }
    
    public void OnGUI()
    {
        // Enter-Key handling:
        if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
        {
            if (!string.IsNullOrEmpty(this.InputLine))
            {
                this.StartChat();
                return;
            }
        }


        GUI.skin.label.wordWrap = true;
        GUILayout.BeginArea(guiCenteredRect);


        if (this.chatComponent != null && string.IsNullOrEmpty(this.chatComponent.ChatAppId))
        {
            GUILayout.Label("To continue, configure your Chat AppId.\nIt's listed in the Chat Dashboard (online).\nStop play-mode and edit:\nScripts/ChatGUI in the Hierarchy.");
            if (GUILayout.Button("Open Chat Dashboard"))
            {
                Application.OpenURL("https://www.exitgames.com/en/Chat/Dashboard");
            }
            GUILayout.EndArea();
            return;
        }

        GUILayout.Label(this.helpText);

        GUILayout.BeginHorizontal();
        GUI.SetNextControlName("NameInput");
        this.InputLine = GUILayout.TextField(this.InputLine);
        if (GUILayout.Button("Connect", GUILayout.ExpandWidth(false)))
        {
            this.StartChat();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndArea();


        GUI.FocusControl("NameInput");
    }

    private void StartChat()
    {
        this.chatComponent.UserName = this.InputLine;
        this.chatComponent.enabled = true;
        this.enabled = false;

        PlayerPrefs.SetString(NamePickGui.UserNamePlayerPref, this.InputLine);
    }
}
