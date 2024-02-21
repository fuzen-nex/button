using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.GameElements;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay
{
    public enum QuestionMode
    {
        ColorOnly,
        ShapeOnly,
        ColorAndShape
    }

    [Serializable]
    public class QueryAudioClip
    {
        public string query;
        public AudioClip clip;
    }
    public class Question
    {
        public int Answer;
        public string QueryString;
        public List<Color> Colors;
        public List<SignShape> Shapes;
        public AudioSource QueryAudioSource;
    }
    
    public class QuestionManager : MonoBehaviour
    {
        private readonly List<SignShape> allShapes = new() {SignShape.Circle, SignShape.Rectangle, SignShape.Square, SignShape.Triangle};
        private readonly List<Color> allColors = new() { Color.blue, Color.red, Color.green, Color.yellow };
        [SerializeField] private List<QueryAudioClip> queryAudioClips;
        [SerializeField] private AudioSource audioSource;

        private static readonly Dictionary<Color, string> ColorDict = new()
        {
            {Color.blue, "blue"},
            {Color.red, "red"},
            {Color.yellow, "yellow"},
            {Color.green, "green"}
        };

        public Question GenerateQuestion(int numberOfChoices, QuestionMode mode)
        {
            Shuffle();
            var question = new Question
            {
                Shapes = new List<SignShape>(),
                Colors = new List<Color>(),
                QueryAudioSource = new AudioSource()
            };
            for (var i = 0; i < numberOfChoices; i++)
            {
                question.Colors.Add(allColors[i]);
                question.Shapes.Add(allShapes[i]);
            }
            question.Answer = Random.Range(0, numberOfChoices);
            question.QueryString = GetAsk(mode, question.Colors[question.Answer], question.Shapes[question.Answer]);
            
            foreach (var queryAudioClip in queryAudioClips.Where(queryAudioClip => queryAudioClip.query == question.QueryString))
            {
                audioSource.clip = queryAudioClip.clip;
            }
            question.QueryAudioSource = audioSource;
            return question;
        }

        private string GetAsk(QuestionMode mode, Color color, SignShape shape)
        {
            switch (mode)
            {
                case QuestionMode.ColorOnly:
                    return GetStringFromColor(color);
                case QuestionMode.ShapeOnly:
                    return GetStringFromShape(shape);
                case QuestionMode.ColorAndShape:
                {
                    var side = Random.Range(0, 2);
                    return side == 0 ? GetStringFromColor(color) : GetStringFromShape(shape);
                }
                default:
                    return "error";
            }
        }

        private static string GetStringFromColor(Color color)
        {
            return ColorDict[color];
        }

        private static string GetStringFromShape(SignShape shape)
        {
            return shape switch
            {
                SignShape.Circle => "circle",
                SignShape.Rectangle => "rectangle",
                SignShape.Square => "square",
                SignShape.Triangle => "triangle",
                _ => "error"
            };
        }

        private void Shuffle()
        {
            for (var i = 1; i < allShapes.Count; i++)
            {
                var p = Random.Range(0, i + 1);
                (allShapes[p], allShapes[i]) = (allShapes[i], allShapes[p]);
            }
            for (var i = 1; i < allColors.Count; i++)
            {
                var p = Random.Range(0, i + 1);
                (allColors[p], allColors[i]) = (allColors[i], allColors[p]);
            }
        }
    }
}
