using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PFCToolkit.Lib
{
    public class ErrorLog
    {
        public ScrollView view = new ScrollView();
        public ErrorLog()
        {
            StyleSheet styleSheet = Resources.Load<StyleSheet>("PFCToolkit/Notes");
            view.styleSheets.Add(styleSheet);
            view.AddToClassList("NoteLog");
        }

        public LogNote AddNote(string title, string description, noteType type = noteType.log)
        {
            LogNote note = new LogNote(title, description, type);
            view.Add(note);
            return note;
        }

        public void RemNote(LogNote note)
        {
            view.Remove(note);
        }

        public void ClearLog()
        {
            view.Clear();
        }
    }

    public class LogNote : VisualElement
    {
        private Label _title;
        private TextElement _desc;

        public string title { get { return _title.text; } set { _title.text = value; } }
        public string desciption { get { return _desc.text; } set { _desc.text = value; } }

        public LogNote(string name, string description, noteType type = noteType.log)
        {
            Debug.Log("Made note of type " + type.ToString());
            this.AddToClassList("notification");
            this.AddToClassList(type.ToString());
            _title = new Label(name);
            _title.AddToClassList("title");
            _desc = new TextElement() { text = description };
            _desc.AddToClassList("description");

            this.Add(_title);
            this.Add(_desc);
        }
    }
    public enum noteType
    {
        log,
        warning,
        error
    }
}