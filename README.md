# Vandelay Industries - Fine Latex Products - A module for Orchard CMS that does lots of stuff

This suite of Orchard modules contains the following features:
Classy, Custom Sort Favicon, Meta tags, Splash Screen, Theme Picker and Translation Manager.

## Classy

Add ids, classes and scripts to the rendering of your content items through this part that can be added to any content type.
To use, go into content/content types, edit a content type and add the Custom CSS part.

The content template for the content type may have to be changed so that it renders the additional attributes.
See http://orchard.codeplex.com/discussions/259938.

## Favicon

This feature adds a favicon configuration page to site settings.
Favicons should be stored into a favicon subfolder of the media folder.

## Meta Tags

Add the Meta content part to Page or to any of your content types to add keywords and description meta tags. 

*Warning:* keywords and descriptions are no SEO silver bullets and can even be counter-productive.
This will not magically get your site to #1 in search engines. The best SEO techniques are to write relevant contents,
format it with valid markup under friendly URLs.

##Remote RSS

Include remote RSS feeds into your site using a widget or a content part.

## Theme Picker

The Theme Picker provides an admin-based way of picking one of the currently active themes for different user agents.
For example, you can provide a mobile theme for your visitors who are using their phone's browser. Optionally,
the module can add to each page a templatable piece of UI that enables the user to force the use of the default theme,
for example to enable phone users to still use the site's full desktop experience.

The admin page for the theme picker can be found under the Picker tab of the Themes admin section.

## Translation Manager

A bunch of command-line tools that facilitate the creation and management of Orchard translation files.

* `install translation <translationZip>`
  Installs the translation files contained in the zip file. Translations for modules that are not physically present
  in the application are skipped.
* `package translation /Culture:<cultureCode> /Extensions:<extensionName1>,<extensionName2>... /Input:<inputDirectory> /Output:<outputDirectory>`
  Packages translation files for the specified culture into a zip file in the specified output directory.
  One or more module or theme names can be specified, in which case only the po files for those modules
  and themes are included in the package. If no extension name is specified, all modules are scanned for
  po files, as well as Core and App_Data.
* `extract default translation /Extensions:<extensionName1>,<extensionName2>... /Input:<inputDirectory> /Output:<outputDirectory>`
  Extracts and packages a translation file for the default culture.
* `sync translation /Input:<translationDirectory> /Culture:<cultureCode>`
  Synchronizes the translation for the specified culture with the default translation.
  If the translation for the specified culture does not exist yet, this generates a stub for it.
  The Output switch must point to a directory that contains both translations.
