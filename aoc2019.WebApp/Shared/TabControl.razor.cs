using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace aoc2019.WebApp.Shared
{
    /// <summary>
    /// Tabbed container control with a single active page.
    /// Add pages using <see cref="TabPage"/> items as child content.
    /// </summary>
    public sealed partial class TabControl : ComponentBase
    {
        /// <summary>
        /// The content of the TabControl, consisting of <see cref="TabPage"/> items.
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// The active page of the TabControl.
        /// </summary>
        public TabPage ActivePage { get; set; }

        /// <summary>
        /// The pages of the TabControl.
        /// </summary>
        public IReadOnlyList<TabPage> Pages => myPages;

        /// <summary>
        /// Show the given page of the TabControl.
        /// </summary>
        /// <param name="page">The page to activate, existing within <see cref="Pages"/>.</param>
        public void ActivatePage(TabPage page) => ActivePage = page;

        /// <summary>
        /// Add a page to the TabControl.
        /// </summary>
        /// <param name="page">The page o be added.</param>
        internal void RegisterPage(TabPage page)
        {
            myPages.Add(page);
            StateHasChanged();
        }

        private readonly List<TabPage> myPages = new List<TabPage>();
    }
}
