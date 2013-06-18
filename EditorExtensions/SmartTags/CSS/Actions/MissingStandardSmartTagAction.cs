﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media.Imaging;
using Microsoft.CSS.Core;
using Microsoft.CSS.Editor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.Web.Editor;
using Microsoft.Web.Editor.Formatting;

namespace MadsKristensen.EditorExtensions
{
    internal class MissingStandardSmartTagAction : CssSmartTagActionBase
    {
        private ITrackingSpan _span;
        private Declaration _declaration;
        private string _standardName;
        private ITextView _view;

        public MissingStandardSmartTagAction(ITrackingSpan span, Declaration declaration, string standardName, ITextView view)
        {
            _span = span;
            _declaration = declaration;
            _standardName = standardName;
            _view = view;

            if (Icon == null)
            {
                Icon = BitmapFrame.Create(new Uri("pack://application:,,,/WebEssentials2013;component/Resources/warning.png", UriKind.RelativeOrAbsolute));
            }
        }

        public override string DisplayText
        {
            get { return string.Format(CultureInfo.InvariantCulture, Resources.StandardSmartTagActionName, _standardName); }
        }

        public override void Invoke()
        {
            string separator = _declaration.Parent.Text.Contains("\r") || _declaration.Parent.Text.Contains("\n") ? Environment.NewLine : " ";
            int index = _declaration.Text.IndexOf(":", StringComparison.Ordinal);
            string newDec = _standardName + _declaration.Text.Substring(index);

            EditorExtensionsPackage.DTE.UndoContext.Open(DisplayText);
            SnapshotSpan span = _span.GetSpan(_span.TextBuffer.CurrentSnapshot);
            _span.TextBuffer.Replace(span, _declaration.Text + separator + newDec);
            FormatItem(_declaration.Start);
            EditorExtensionsPackage.DTE.UndoContext.Close();
        }

        private void FormatItem(int start)
        {
            IEnumerable<Lazy<IEditorFormatter>> providers = ComponentLocator<IEditorFormatter>.ImportMany();
            foreach (var locator in providers)
            {
                Span span = new Span(start, 1);
                locator.Value.Format(_view, _view.TextBuffer);
                break;
            }
        }
    }
}
