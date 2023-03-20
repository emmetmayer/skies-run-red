using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public static class NetworkMain
{
    public static void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // The client identifier to be authenticated
        var clientId = request.ClientNetworkId;

        // Additional connection data defined by user code
        var connectionData = request.Payload;

        // Your approval logic determines the following values
        response.Approved = true;
        response.CreatePlayerObject = false;

        AddClientAsAgent(clientId);

        // If additional approval steps are needed, set this to true until the additional steps are complete
        // once it transitions from true to false the connection approval response will be processed.
        response.Pending = false;
    }

    public static void AddClientAsAgent(ulong clientId)
    {
        Agent agent = AgentService.Instance.AddAgent(clientId, "PLAYER_1");
        TeamService.Instance.SpawnAgent(agent);
    }
}
