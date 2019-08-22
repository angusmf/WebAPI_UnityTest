using UnityEngine;
using IngameDebugConsole;
using UniRx;
using UnityEngine.Networking;

public class ConsoleCommands : MonoBehaviour
{
    void Start()
    {
        DebugLogConsole.AddCommandInstance("cube", "Creates a cube at [0 2.5 0]", "CreateCubeAt", this);

        DebugLogConsole.AddCommandInstance("server", "Call NetworkManager.singleton.StartServer", "StartServer", this);

        DebugLogConsole.AddCommandInstance("client", "Call NetworkManager.singletone.StartClient", "StartClient", this);
    }

    public static void CreateCubeAt(Vector3 position)
    {
        if (position == null)
        {
            Debug.Log("position is null");
            position = new Vector3(0, 0, 0);
        }else
        {
            Debug.Log("Position is not null");
        }
        GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = position;
    }

    public void StartServer()
    {
        MessageBroker.Default.Publish(new StartServer());
    }

    public void StartClient()
    {
        MessageBroker.Default.Publish(new StartClient());
    }
}