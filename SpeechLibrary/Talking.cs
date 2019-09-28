using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech;
using System.Speech.Recognition;

namespace SpeechLibrary
{
    using System.Speech.Synthesis;

    public class Talking
    {
        private  SpeechSynthesizer speechSynthesizer;

        private  PromptBuilder promptBuilder;

        private  SpeechRecognitionEngine speechRecognitionEngine;
        public Talking()
        {
            this.speechSynthesizer = new SpeechSynthesizer();
            this.speechSynthesizer.SetOutputToDefaultAudioDevice();
            this.promptBuilder = new PromptBuilder();
            this.speechRecognitionEngine = new SpeechRecognitionEngine();

        }

        public  void SpeakText(string strToSpeak)
        {
            this.promptBuilder.ClearContent();
            this.promptBuilder.AppendText(strToSpeak);
            this.speechSynthesizer.Speak(this.promptBuilder);
        }
    }
}
