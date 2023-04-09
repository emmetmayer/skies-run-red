using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameTagScript : MonoBehaviour
{
    private string _storedName;
    [SerializeField] private TMP_Text _tag;
    [SerializeField] private RawImage _teamColor;
    [SerializeField] private AgentCharacter _char;
    [SerializeField] private Canvas _canvasRef;
    // Start is called before the first frame update
    public void OnNametagReady()
    {
        _storedName = _char.m_Agent.GetName();
        _tag.text = _storedName;
        _teamColor.color = _char.m_Agent.m_TeamID.Value == 0 ? Color.blue : Color.red;
    }

    // Update is called once per frame
    //https://forum.unity.com/threads/making-canvas-in-world-facing-you-but-not-the-camera-center.1059581/
    void Update()
    {
        Quaternion lookRotation = Camera.main.transform.rotation;
        _canvasRef.transform.rotation = lookRotation;
    }
}
