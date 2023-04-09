using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class NetworkMain : NetworkBehaviour
{
    public static NetworkMain Instance;

    public void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // The client identifier to be authenticated
        var clientId = request.ClientNetworkId;

        // Additional connection data defined by user code
        var connectionData = request.Payload;

        // Your approval logic determines the following values
        response.Approved = true;
        response.CreatePlayerObject = false;
        
        // If additional approval steps are needed, set this to true until the additional steps are complete
        // once it transitions from true to false the connection approval response will be processed.
        response.Pending = false;
    }


    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayerAgentServerRpc(FixedString64Bytes playerNameFixed, int playerTeam, ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        string playerName = playerNameFixed.ToString();
        Agent agent = CTF.AgentService.AddAgent(clientId, playerName.ToString(), playerTeam);
        agent.LoadCharacter();
    }

    [ClientRpc]
    public void AskPlayerToPrepareSpawnRequestClientRpc(ClientRpcParams clientRpcParams = default)
    {
        FixedString64Bytes playerNameFixed = LobbyManager.Instance.playerName;
        int playerTeam = int.Parse(LobbyManager.Instance.playerTeam);
        SpawnPlayerAgentServerRpc(playerNameFixed, playerTeam);
    }

    public void AskPlayerToPrepareSpawnRequest(ulong clientId)
    {
        AskPlayerToPrepareSpawnRequestClientRpc(new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        });
    }


    public void ConnectedCallback(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            if (CTF.Instance.IsRunning.Value)
            {
                AskPlayerToPrepareSpawnRequest(clientId);
            }
            else
            {
                CTF.Instance.IsRunning.OnValueChanged += (bool previous, bool current) => {
                    if (!previous && current)
                    {
                        AskPlayerToPrepareSpawnRequest(clientId);
                    }
                };
            }
        }
    }

    void Awake()
    {
        Instance = this;
    }
}
