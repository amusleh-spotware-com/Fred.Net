# Fred.Net
St. Louis Fed API toolkit for .Net

## Nuget

```Install-Package Fred.Net```

## Getting Started

```c#
// Create an instance of client class
Client client = new Client(your_api_key);

// For getting a series observations
List<Observation> result = await client.GetSeriesObservations("GNPCA");
```

You can call all API requests via client class methods and each call will return its data in form of Fred.Net.Type classes.

For using requests options parameters use the keyword arguments of each request method.

The library uses XML format for getting that data.

The target framework is .Net Standard 2.0
