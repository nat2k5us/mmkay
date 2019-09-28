using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechLibrary
{
    using System.Reflection;
    using System.Speech.Recognition;
    using System.Speech.Synthesis;

    using NLog;

    public interface ISpeechRecognizer
    {

      
    }

    public class SpeechRecognizer : ISpeechRecognizer
    {
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly List<InstalledVoice> installedVoices;

        private readonly SpeechRecognitionEngine speechRecognitionEngine = new SpeechRecognitionEngine();

        private readonly SpeechSynthesizer synthesizer = new SpeechSynthesizer();

        private List<string> choicesList;

        public SpeechRecognizer(List<string> choicesList )
        {

            this.choicesList = choicesList;
            try
            {
                this.installedVoices = new List<InstalledVoice>(this.synthesizer.GetInstalledVoices());
                this.synthesizer.Volume = 100; // 0...100
                this.synthesizer.Rate = -2; // -10...10
                this.synthesizer.SelectVoice(this.installedVoices.First().VoiceInfo.Name);
                var numbers = new Choices();
                numbers.Add(choicesList.ToArray()); 
                // Create a GrammarBuilder object and append the Choices object.
                var gb = new GrammarBuilder();
                gb.Append(new GrammarBuilder(numbers), 1, 5);
                var grammer = new Grammar(gb) { Priority = 127 };
                this.speechRecognitionEngine.RequestRecognizerUpdate();
                this.speechRecognitionEngine.LoadGrammar(grammer);
                this.speechRecognitionEngine.SetInputToDefaultAudioDevice();
                this.speechRecognitionEngine.SpeechRecognized += this.SpeechRecognitionEngine_SpeechRecognized;
                this.speechRecognitionEngine.SpeechDetected += this.SpeechRecognitionEngine_SpeechDetected;
                this.speechRecognitionEngine.SpeechRecognitionRejected += this.SpeechRecognitionEngine_SpeechRecognitionRejected;
                this.speechRecognitionEngine.RecognizeCompleted += this.SpeechRecognitionEngine_RecognizeCompleted;
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message + exception.InnerException?.Message);
                throw exception;
            }
            Logger.Info($" Finished audio Initializer with no exceptions - found {this.installedVoices.Count} voices");
        }


        private void SpeechRecognitionEngine_RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
        {
            Logger.Info($" Done with recognition ...");
           this.speechRecognitionEngine.RecognizeAsync(RecognizeMode.Single);
        }

        private void SpeechRecognitionEngine_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Logger.Info($"{MethodBase.GetCurrentMethod().Name} - {e.Result.Text} ");
        }

        private void SpeechRecognitionEngine_SpeechDetected(object sender, SpeechDetectedEventArgs e)
        {
            Logger.Info($"{MethodBase.GetCurrentMethod().Name} - - {e.AudioPosition.TotalMilliseconds}");
        }

        private void SpeechRecognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Logger.Info($" speech recognised : {e.Result.Text}");
        }


    }
}
