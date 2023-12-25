using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private AudioSource audioSource;

    [SerializeField]
    private SerializableDictionary<string, AudioClip> audioClips = new SerializableDictionary<string, AudioClip>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {

        audioSource = GetComponent<AudioSource>();
        audioClips.AddFromInspector();

    }

    public void PlayAudio(string audioName)
    {
        if (audioClips.Dictionary.ContainsKey(audioName))
        {
            audioSource.PlayOneShot(AudioClips[audioName]);
        }
        else
        {
            Debug.LogWarning("Audio clip with the name '" + audioName + "' not found.");
        }

    }

    public void PlayClipAtPoint(string audioName, Vector3 position, float volume = 1f, float minDistance3D = 1f)
    {
        if (audioClips.Dictionary.ContainsKey(audioName))
        {
            GameObject gameObject = new GameObject("One shot audio");
            gameObject.transform.position = position;
            AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
            audioSource.minDistance = minDistance3D;
            audioSource.clip = AudioClips[audioName];
            audioSource.spatialBlend = 1f;
            audioSource.volume = volume;
            audioSource.Play();
            Destroy(gameObject, AudioClips[audioName].length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale) + 5);
        }
        else
        {
            Debug.LogWarning("Audio clip with the name '" + audioName + "' not found.");
        }
    }

    public Dictionary<string, AudioClip> AudioClips
    {
        get { return audioClips.Dictionary; }
    }

}

[Serializable]
public class SerializableDictionary<TKey, TValue>
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();

    [SerializeField]
    private List<TValue> values = new List<TValue>();

    private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

    public void Add(TKey key, TValue value)
    {
        if (!dictionary.ContainsKey(key))
        {
            dictionary.Add(key, value);
        }
        else
        {
            Debug.LogWarning("An item with the same key already exists in the dictionary.");
        }
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return dictionary.TryGetValue(key, out value);
    }

    public Dictionary<TKey, TValue> Dictionary
    {
        get { return dictionary; }
    }

    public void AddFromInspector()
    {
        for (int i = 0; i < keys.Count; i++)
        {
            Add(keys[i], values[i]);
        }
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
public class SerializableDictionaryDrawer : PropertyDrawer
{
    private const float ButtonWidth = 20f;
    private const float Margin = 1f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty keys = property.FindPropertyRelative("keys");
        SerializedProperty values = property.FindPropertyRelative("values");

        int arraySize = keys.arraySize;

        float elementHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        float totalHeight = elementHeight * (arraySize + 2) + EditorGUIUtility.standardVerticalSpacing * (arraySize + 1);

        Rect contentPosition = new Rect(position.x, position.y, position.width, totalHeight);

        EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Ekleme butonu
        Rect addButtonPosition = new Rect(contentPosition.x + contentPosition.width - ButtonWidth, contentPosition.y, ButtonWidth, EditorGUIUtility.singleLineHeight);
        contentPosition.width -= ButtonWidth;

        if (GUI.Button(addButtonPosition, "+"))
        {
            keys.InsertArrayElementAtIndex(keys.arraySize);
            values.InsertArrayElementAtIndex(values.arraySize);
        }

        EditorGUI.indentLevel++;
        contentPosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        // Eleman numarasý, key, value ve çýkarma butonunu içeren grid
        for (int i = 0; i < arraySize; i++)
        {
            Rect elementPosition = new Rect(contentPosition.x, contentPosition.y, contentPosition.width - ButtonWidth - Margin, EditorGUIUtility.singleLineHeight);
            Rect removeButtonPosition = new Rect(elementPosition.x + elementPosition.width + ButtonWidth, contentPosition.y, ButtonWidth, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(elementPosition, i.ToString());
            elementPosition.x += 20f;

            elementPosition.width = (contentPosition.width - ButtonWidth) / 2 - Margin;

            EditorGUI.PropertyField(elementPosition, keys.GetArrayElementAtIndex(i), GUIContent.none);
            elementPosition.x += elementPosition.width + Margin;

            EditorGUI.PropertyField(elementPosition, values.GetArrayElementAtIndex(i), GUIContent.none);
            elementPosition.x += elementPosition.width + Margin;

            // Çýkarma butonu
            if (GUI.Button(removeButtonPosition, "-"))
            {
                keys.DeleteArrayElementAtIndex(i);
                values.DeleteArrayElementAtIndex(i);
                arraySize--;
            }

            contentPosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty keys = property.FindPropertyRelative("keys");
        return EditorGUIUtility.singleLineHeight * (keys.arraySize + 2) + EditorGUIUtility.standardVerticalSpacing * (keys.arraySize + 1);
    }
}
#endif
