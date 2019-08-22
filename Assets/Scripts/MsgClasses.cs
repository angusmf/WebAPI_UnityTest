
public class ClientAuthTokenReceived
{
    public Player player;
    public string token;

    public ClientAuthTokenReceived(string token, Player player)
    {
        this.player = player;
        this.token = token;
    }
}

public class SendAuthTokenToServer
{
    public string tokenVal;
    public SendAuthTokenToServer(string token)
    {
        tokenVal = token;
    }
}

public class PlayerStarted { }

public class AuthTokenReceived
{
    public string tokenVal;
    public AuthTokenReceived(string token)
    {
        tokenVal = token;
    }
}


public class StartServer { }


public class StartClient { }



public class ServerAuthTokenReceived {
    public string tokenVal;
    public ServerAuthTokenReceived(string token)
    {
        tokenVal = token;
    }
}

public class DebugMsg
{
    public string msg;
    public DebugMsg(string msg) { this.msg = msg; }
    
}

public class ErrorMsg
{
    public string msg;
    public ErrorMsg(string msg) { this.msg = msg; }
}

public class ExceptionMsg
{
    public string msg;
    public ExceptionMsg(string msg) { this.msg = msg; }
}




