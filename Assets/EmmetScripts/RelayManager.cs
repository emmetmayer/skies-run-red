using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using Unity.Netcode;

public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance { get; private set; }

    public bool m_HasStartedConnection = false;


    private void Awake()
    {
        Instance = this;
    }

    public async Task<string> CreateRelay(int players)
    {
        try
        {
            if (m_HasStartedConnection)
            {
                throw new System.Exception("Cannot create relay; connection already started!");
            }
            m_HasStartedConnection = true;

            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(players - 1);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.ConnectionApprovalCallback += NetworkMain.ApprovalCheck;
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkMain.ConnectedCallback;
            
            NetworkManager.Singleton.StartHost();

            if (NetworkManager.Singleton.IsServer)
            {
                CTF.Instance.StartGame();
            }
            else
            {
                CTF.Instance.StartGameServerRPC();
            }

            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return null;
        }
    }

    public async void JoinRelay(string joinCode)
    {
        try
        {
            if (m_HasStartedConnection)
            {
                throw new System.Exception("Cannot join relay; connection already started!");
            }
            m_HasStartedConnection = true;

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
}
