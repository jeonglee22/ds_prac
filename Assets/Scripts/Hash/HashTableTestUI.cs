using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HashTableTestUI : MonoBehaviour
{
    private const int DefaultCapacity = 16;

    enum Method
    {
        OpenAdressing,
        ChainingHash,
    }

    public TMP_Dropdown methodDropDown;
    public TMP_Dropdown probingDropDown;

    public TMP_InputField keyInputField;
    public TMP_InputField valueInputField;

    public TextMeshProUGUI hashHistory;

    public Button addButton;
    public Button removeButton;
    public Button clearButton;

    public ScrollRect tableViewRect;
    public GameObject emptySlot;
    public GameObject occupiedSlot;
    public GameObject indexBlock;

    private Method currentMethod = Method.OpenAdressing;

    private OpenAdressingHashTable<string, int> openHashTable;
    private ChainingHashTable<string, int> chainingHashTable;

    private string inputKey;
    private int inputValue;

    private bool isCleared;

    private RectTransform content;

    private List<GameObject> visualObjs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        content = tableViewRect.content;

        openHashTable = new OpenAdressingHashTable<string, int>();
        chainingHashTable = new ChainingHashTable<string, int>();

        visualObjs = new List<GameObject>();

        methodDropDown.onValueChanged.AddListener((i) => OnMethodValueChanged(i));
        probingDropDown.onValueChanged.AddListener((i) => OnProbingValueChanged(i));

        keyInputField.onValueChanged.AddListener(s => OnKeyFieldChanged(s));
        valueInputField.onValueChanged.AddListener(s => OnValueFieldChanged(s));

        addButton.onClick.AddListener(() => OnAddKVPClicked());
        removeButton.onClick.AddListener(() => OnRemoveKVPClicked());
        clearButton.onClick.AddListener(() => OnClearKVPClicked());

        isCleared = true;

        ResetVisualObjs();
    }
    
    private void ResetVisualObjs()
    {
        visualObjs.Clear();
        for (int i = 0; i < content.childCount; i++)
        {
            var child = content.GetChild(i);
            Destroy(child.gameObject);
        }

        for (int i = 0; i < DefaultCapacity; i++)
        {
            var obj = Instantiate(indexBlock, content);
            Instantiate(emptySlot, obj.transform);
            visualObjs.Add(obj);
            SetEmptyText(obj, i);
        }
    }

    private void OnMethodValueChanged(int index)
    {
        if (!isCleared) return;

        currentMethod = (Method)index;
    }

    private void OnProbingValueChanged(int index)
    {
        if (!isCleared) return;

        openHashTable.ProbingStrategy = (ProbingStrategy)index;
    }

    private void OnKeyFieldChanged(string key)
    {
        inputKey = key;
    }

    private void OnValueFieldChanged(string value)
    {
        inputValue = int.Parse(value);
    }

    private void OnAddKVPClicked()
    {
        isCleared = false;

        var kvp = new KeyValuePair<string, int>(inputKey, inputValue);

        switch (currentMethod)
        {
            case Method.OpenAdressing:
                openHashTable.Add(kvp);
                CheckAddAndUpdateSlot(kvp, occupiedSlot, false);
                break;
            case Method.ChainingHash:
                CheckChainAddAndUpdateSlot(kvp);
                break;
        }
    }

    private void CheckAddAndUpdateSlot(KeyValuePair<string, int> kvp, GameObject slot, bool isRemove)
    {
        var index = openHashTable.GetProbeIndex(inputKey, inputValue);
        Destroy(visualObjs[index].transform.GetChild(0).gameObject);
        var obj = Instantiate(slot, visualObjs[index].transform);
        if (!isRemove)
            SetSlotText(obj, index, inputKey, inputValue);
        else
            SetEmptyText(obj, index);
    }

    private void CheckChainAddAndUpdateSlot(KeyValuePair<string, int> kvp)
    {
        var index2 = chainingHashTable.GetIndex(inputKey);

        bool destroyEmpty = false;
        if (!chainingHashTable.ContainsKey(inputKey))
        {
            destroyEmpty = true;
        }

        chainingHashTable.Add(kvp);

        if (chainingHashTable.ContainsKey(inputKey))
        {
            if (destroyEmpty)
                Destroy(visualObjs[index2].transform.GetChild(0).gameObject);

            var obj = Instantiate(occupiedSlot, visualObjs[index2].transform);
            SetSlotText(obj, index2, inputKey, inputValue);
        }
    }
    
    private void CheckChainRemoveAndUpdateSlot(KeyValuePair<string, int> kvp)
    {
        var index2 = chainingHashTable.GetIndex(inputKey);

        if(chainingHashTable.Remove(kvp));

        if (!chainingHashTable.ContainsKey(inputKey))
        {
            var list = chainingHashTable.GetlistForKey(kvp.Key);

            
            Destroy(visualObjs[index2].transform.GetChild(0).gameObject);
            var obj = Instantiate(emptySlot, visualObjs[index2].transform);
            SetEmptyText(obj, index2);
        }
        else
        {
            
        }
    }

    private void OnRemoveKVPClicked()
    {
        isCleared = false;

        var kvp = new KeyValuePair<string, int>(inputKey, inputValue);

        switch (currentMethod)
        {
            case Method.OpenAdressing:
                if(openHashTable.Remove(kvp))
                    CheckAddAndUpdateSlot(kvp, emptySlot, true);
                break;
            case Method.ChainingHash:
                chainingHashTable.Remove(kvp);
                break;
        }
    }

    private void OnClearKVPClicked()
    {
        openHashTable.Clear();
        chainingHashTable.Clear();

        ResetVisualObjs();
    }

    private void SetHistoryText()
    {

    }

    private void SetSlotText(GameObject obj, int index, string key, int value)
    {
        obj.GetComponentInChildren<TextMeshProUGUI>().text = $"I: {index} K: {key}\t V:{value}";
    }

    private void SetEmptyText(GameObject obj, int index)
    {
        obj.GetComponentInChildren<TextMeshProUGUI>().text = $"I: {index}";
    }
}
