# Bloat Editor

A powerful Unity inspector enhancement package that provides conditional field visibility and other editor utilities.

## Features

- **ShowIf Attribute**: Conditionally show/hide fields based on other field values
- Support for boolean and enum conditions
- Expression-based evaluation

## Installation

1. Copy the `com.yourcompany.bloateditor` folder to your project's `Packages` directory
2. Unity will automatically detect and import the package

## Usage

```csharp
[SerializeField] private bool enableFeature;
[SerializeField, ShowIf("enableFeature")] private float featureValue;

[SerializeField] private MyEnum enumValue;
[SerializeField, ShowIf("enumValue == MyEnum.Option1")] private int option1Value;
```
