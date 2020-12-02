module Styles

open Zanaptak.TypedCssClasses

// Font-Awesome classes
type FA = CssClasses<"https://use.fontawesome.com/releases/v5.8.1/css/all.css", Naming.PascalCase>

// Bulma classes
type Bulma = CssClasses<"https://cdnjs.cloudflare.com/ajax/libs/bulma/0.7.4/css/bulma.min.css", Naming.PascalCase>

// Custom classes
type Style = CssClasses<"public/styles.css", Naming.PascalCase, resolutionFolder=__SOURCE_DIRECTORY__>
