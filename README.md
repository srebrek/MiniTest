<a id="readme-top"></a>
<!-- Made using othneildrew's template -->

<!-- PROJECT LOGO -->
<br />
<div align="center">

  <h3 align="center">MiniTest</h3>

  <p align="center">
    A lightwieight C# unit test framework.
  </p>
</div>



<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

A lightweight, minimalistic unit testing framework for C# applications. MiniTestRunner provides a simple and effective way to define, execute, and report unit tests without relying on third-party test frameworks like NUnit or xUnit.

Features: 
* Lightweight and easy to integrate into any C# project
* Automatically discovers and runs test methods
* Simple assertion mechanism for validating test results
* Clear and structured test output
* Supports multiple test cases in a single run
* Can be expanded with custom test attributes and reporting


## How it Works

* The framework scans assemblies for test methods based on attributes like `[Testmethod]`.
* Each test method runs independently, capturing results and failures.
* The test runner provides a summary report of passed and failed tests.

<p align="right">(<a href="#readme-top">back to top</a>)</p>



### Built With

This section should list any major frameworks/libraries used to bootstrap your project. Leave any add-ons/plugins for the acknowledgements section. Here are a few examples.

* [![.NET 8.0][dotnet-shield]][dotnet-url]
* [![C#][csharp-shield]][csharp-url]
* [![Visual Studio][vs-shield]][vs-url]

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- GETTING STARTED -->
## Getting Started

There is a demonstration project `AuthenticationService` and its testing project `AuthenticationService.Tests` included which will be set up in tutorial below.

### Installation

1. Clone the repo
   ```sh
   git clone https://github.com/srebrek/MiniTest.git
   ```
2. Build `MiniTestRunner` and `AuthenticationService.Tests`.
3. Run `MiniTestRunner.exe` without arguments. (double left click)

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- USAGE EXAMPLES -->
## Usage

In order to test your own projects, simply create a test project with a reference to the tested project, create a class with `TestClass` attribute. All methods inside `TestClass` with `TestMethod` attribute will be run by `MiniTestRunner.`

More information included in documentation.

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE` for more information.

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->

[dotnet-shield]: https://img.shields.io/badge/.NET%208.0-512BD4?style=for-the-badge&logo=.net&logoColor=white
[dotnet-url]: https://dotnet.microsoft.com/en-us/download/dotnet/8.0
[csharp-shield]: https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white
[csharp-url]: https://learn.microsoft.com/en-us/dotnet/csharp/
[vs-shield]: https://img.shields.io/badge/Visual%20Studio-5C2D91?style=for-the-badge&logo=visual-studio&logoColor=white
[vs-url]: https://visualstudio.microsoft.com/