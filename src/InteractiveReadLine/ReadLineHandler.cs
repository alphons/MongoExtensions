// https://github.com/mattj23/InteractiveReadLine
using System.Text;
using InteractiveReadLine.Formatting;
using InteractiveReadLine.KeyBehaviors;
using InteractiveReadLine.Tokenizing;

#nullable disable

namespace InteractiveReadLine
{
    /// <summary>
    /// This class handles getting a single line of input from the underlying IReadLineProvider. It reads keys from the
    /// provider, determines what the current line of text being edited should be and where the cursor should be positioned.
    /// It pushes out the text to the provider, which also serves as the view.
    /// </summary>
    public class ReadLineHandler(IReadLineProvider provider, ReadLineConfig config = null) : IKeyBehaviorTarget
    {
		private readonly ReadLineConfig config = config ?? ReadLineConfig.Basic;
        private int cursorPos = 0;
        private int autoCompleteIndex = int.MinValue;
        private TokenizedLine autoCompleteTokens;
        private bool autoCompleteCalled = false;
        private string[] autoCompleteSuggestions = null;

        private int historyIndex = config?.History?.Any() == true ? config.History.Count : 0;
        private LineState preHistoryState;

        private bool finishTrigger = false;

		/// <summary>
		/// Gets the current LineState representation of the text and the cursor position
		/// </summary>
		public LineState LineState => new(TextBuffer.ToString(), cursorPos);

		/// <inheritdoc />
		public StringBuilder TextBuffer { get; } = new StringBuilder();

		/// <inheritdoc />
		public int CursorPosition
        {
            get => cursorPos;
            set
            {
                cursorPos = value;
                if (cursorPos > TextBuffer.Length)
                    cursorPos = TextBuffer.Length;
                if (cursorPos < 0)
                    cursorPos = 0;
            }
        }

        /// <inheritdoc />
        public ConsoleKeyInfo ReceivedKey { get; private set; }

        /// <inheritdoc />
        public void AutoCompleteNext()
        {
            if (autoCompleteIndex >= 0)
            {
                // Next index
                autoCompleteIndex++;
                if (autoCompleteIndex >= autoCompleteSuggestions.Length)
                    autoCompleteIndex = 0;

                this.SetAutoCompleteText();
            }
            else
                this.StartAutoComplete();
        }

        /// <inheritdoc />
        public void AutoCompletePrevious()
        {
            if (autoCompleteIndex >= 0)
            {
                // Previous index
                autoCompleteIndex--;
                if (autoCompleteIndex < 0)
                    autoCompleteIndex = autoCompleteSuggestions.Length - 1;

                this.SetAutoCompleteText();
            }
            else 
                this.StartAutoComplete();

        }

        /// <inheritdoc />
        public void InsertText(FormattedText text)
        {
            provider.InsertText(text);
        }

        /// <inheritdoc />
        public TokenizedLine GetTextTokens()
        {
            return config.Lexer?.Invoke(this.LineState);
        }

        public void HistoryNext()
        {
            // If there is no history, we don't need to do anything
            if (config.History?.Any() != true)
                return;

            // If we're at the end of the history (including the entered text) we do nothing
            if (historyIndex == config.History.Count)
                return; 

            // Otherwise we increment the history index and set the current text buffer based 
            // on whether or not we still have another history element
            historyIndex++;
            this.TextBuffer.Clear();
            if (historyIndex == config.History.Count)
            {
                this.TextBuffer.Append(preHistoryState.Text);
                this.CursorPosition = preHistoryState.Cursor;
            }
            else
            {
                this.TextBuffer.Append(config.History[historyIndex]);
                this.CursorPosition = this.TextBuffer.Length;
            }
        }

        public void HistoryPrevious()
        {
            // If there is no history, we don't need to do anything
            if (config.History?.Any() != true)
                return;

            if (historyIndex == 0)
                return;

            // Check if we're about to leave entered text to go backwards in the history. If so
            // we want to store it first.
            if (historyIndex == config.History.Count)
            {
                preHistoryState = new LineState(this.LineState.Text, this.LineState.Cursor);
            }

            historyIndex--;
            this.TextBuffer.Clear();
            this.TextBuffer.Append(config.History[historyIndex]);
            this.CursorPosition = this.TextBuffer.Length;
        }

        /// <summary>
        /// Interactively manage the user input of a line of text at the console, returning the contents
        /// of the text when finished.
        /// </summary>
        public string ReadLine()
        {
            // The display must be updated at the beginning if any prompts or other prefix/suffix text
            // is to be displayed 
            this.UpdateDisplay();

            // The main processing loop of the handler, this loop will block until it receives a single key from the
            // console. It will then attempt to look up a key behavior for that key, and if it finds one it will 
            // invoke it, otherwise it will invoke the default behavior if there is one. After that it will check
            // if the condition to finish the input has been set, and if not it will update the display and wait
            // for the next key.
            while (true)
            {
                this.ReceivedKey = provider.ReadKey();

                // We will need to check if the line state (text & cursor position) is altered by the
                // key behavior which will be run, so we store the current state now
                var previousState = this.LineState;
                autoCompleteCalled = false;
                
                // See if there's a specific behavior which should be mapped to this key,
                // and if so, run it instead of checking the insert/enter behaviors
                var behavior = this.GetKeyAction(ReceivedKey);
                if (behavior != null)
                {
                    behavior.Invoke(this);
                }
                else
                {
                    config.DefaultKeyBehavior?.Invoke(this);
                }

                // Check if the Finish behavior was called, indicating that we can exit this method
                // and return the contents of the text buffer to the caller
                if (finishTrigger)
                    break;

                // If the text contents or the cursor have changed at all, and we weren't currently
                // doing autocomplete, we need to invalidate the auto-completion information
                if ((!previousState.Equals(this.LineState)) && !autoCompleteCalled)
                    this.InvalidateAutoComplete();

                this.UpdateDisplay();
            }

            // If there is a delegate to update the history, invoke it now
            config.UpdateHistory?.Invoke(TextBuffer.ToString());

            return TextBuffer.ToString();
        }

        /// <summary>
        /// Updates the display on the underlying provider. This is where any formatter is called, immediately
        /// prior to the display being set.
        /// </summary>
        private void UpdateDisplay()
        {
            // Finally, if we have an available formatter, we can get a display format from here
            var display = new LineDisplayState(string.Empty, TextBuffer.ToString(), string.Empty, cursorPos);

            if (config.FormatterFromLine != null)
                display = config.FormatterFromLine.Invoke(LineState);
            else if (config.FormatterFromTokens != null && config.Lexer != null)
                display = config.FormatterFromTokens(GetTextTokens());

            provider.SetDisplay(display);
        }

        /// <summary>
        /// Check a ConsoleKeyInfo to see if the configuration object has a behavior registered
        /// for that key. The character is checked first, and if that fails, the ConsoleKey and the modifier keys
        /// are checked. If that fails, null is returned
        /// </summary>
        private Action<IKeyBehaviorTarget> GetKeyAction(ConsoleKeyInfo info)
        {
                var charKey = new KeyId(info.KeyChar);
            if (config.KeyBehaviors.TryGetValue(charKey, out Action<IKeyBehaviorTarget> value1))
                return value1;

            var key = new KeyId(info.Key, (info.Modifiers & ConsoleModifiers.Control) != 0,
                (info.Modifiers & ConsoleModifiers.Alt) != 0, (info.Modifiers & ConsoleModifiers.Shift) != 0);
            if (config.KeyBehaviors.TryGetValue(key, out Action<IKeyBehaviorTarget> value2))
                return value2;

            return null;
        }

        /// <summary>
        /// Initializes and begins the AutoComplete functionality. AutoComplete is its own special state of interaction,
        /// and must be initialized before the next/previous functions will work. It then can continue until an action
        /// other than next/previous occurs, after which it will have to be reinitialized to be used again. The
        /// initialization process involves fetching the valid suggestions for the currently entered text and cursor
        /// position.
        /// </summary>
        private void StartAutoComplete()
        {
            if (!config.CanAutoComplete)
                return;

            autoCompleteTokens = config.Lexer(new LineState(TextBuffer.ToString(), cursorPos));
            if (autoCompleteTokens.CursorToken == null)
                return;

            autoCompleteSuggestions = config.AutoCompletion(autoCompleteTokens) ?? [];

            if (autoCompleteTokens.Text != TextBuffer.ToString())
            {
                TextBuffer.Clear();
                TextBuffer.Append(autoCompleteTokens.Text);
                CursorPosition = autoCompleteTokens.Cursor;
            }

            if (autoCompleteSuggestions.Length != 0)
            {
                autoCompleteIndex = 0;
                SetAutoCompleteText();
            }

        }

        /// <summary>
        /// If AutoComplete is currently active, the only allowable actions are next/previous. If any other
        /// modification is made to the line, the current AutoComplete suggestions are invalidated and any attempt
        /// to use the next/previous functionality will require a reinitialization of the AutoComplete mechanism.
        /// </summary>
        private void InvalidateAutoComplete()
        {
            autoCompleteIndex = Int32.MinValue;
            autoCompleteTokens = null;
            autoCompleteSuggestions = null;
        }

        /// <summary>
        /// Inserts the text from the currently selected auto complete suggestion into the token under the cursor. Only
        /// works if the system is currently in autocomplete mode.
        /// </summary>
        private void SetAutoCompleteText()
        {
            if (!config.CanAutoComplete || autoCompleteTokens == null || autoCompleteIndex < 0)
                return;

            autoCompleteCalled = true;
            autoCompleteTokens.CursorToken.Text = autoCompleteSuggestions[autoCompleteIndex];
            autoCompleteTokens.CursorToken.Cursor = autoCompleteTokens.CursorToken.Text.Length;

            TextBuffer.Clear();
            TextBuffer.Append(autoCompleteTokens.Text);
            CursorPosition = autoCompleteTokens.Cursor;

        }

        /// <summary>
        /// Causes the ReadLine handler to finish, returning the contents of the text buffer
        /// </summary>
        public void Finish() => finishTrigger = true;
    }
}