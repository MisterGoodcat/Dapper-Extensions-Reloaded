# Introduction

Dapper Extensions Reloaded started as a fork of [Dapper Extensions](https://github.com/tmsmith/Dapper-Extensions) but evolved into a stand-alone project over the last years, and diverged from the original in lots of ways. It complements [Dapper](https://github.com/SamSaffron/dapper-dot-net) by adding generic repository features and a predicate system, but tries to stay concise and focused with an attribute-based approach. It avoids convention-based details and too much "black magic" behind the scenes. Dapper Extensions Reloaded also adds a sophisticated logging system. The project is SQL Server-only.

For the full history and credit to the original authors, please take a look at the [contributors](https://github.com/MisterGoodcat/Dapper-Extensions-Reloaded/graphs/contributors) and of course at the original project sites linked above.

# Features

TODO

# Installation

http://nuget.org/List/Packages/DapperExtensionsReloaded

```
PM> Install-Package DapperExtensionsReloaded
```

or 

```
dotnet add package DapperExtensionsReloaded
```

# Usage

TODO

# Change Log

3.2.0
* Update Dapper to 1.6.
* Update other dependencies (test projects)
* Minor code clean-up and removal of unused code.

3.1.2
* Fix handling of DBNull values in standard logger.

3.1.1
* Fix only call dispose of wrapped resources when applicable.

3.1.0
* Make MonitoringDbConnection type as well as its MonitoredDbConnection public to enable scenarios that need to access the underlying connection directly. Example: use bulk insert of SqlConnection.

3.0.0
* Add new logging feature based on proxies of connections and commands.
* Add set-based update operation, rename set-based delete operation.
* Add feature to define key columns by respective attribute.
* Add overload to get a single entity with predicate argument to avoid excessive use of reflection. 
* Convert remaining APIs to async, remove synchronous APIs.
* Remove magic that arbitrarily chooses identity and key columns.
* Remove automatic GUID handling magic.
* Remove implicit object to predicate conversion magic.
* Remove logger in preparation of better implementation.
* Remove unused mapper features and semantics from configuration.
* Fix Xunit warnings and rework tests to make proper use of async features.
* Changed target framework to .NET Standard 2.0.
* Update all dependencies to latest version.
* More code clean-up.

2.0.0
* Changed target framework to .NET Standard 1.3
* Migrate unit tests to Xunit
* Minor code cleanup
* Move to VS2017 project format

1.6.3
* Add possibility to provide logger for SQL statements and parameters
* Add extension method to format DynamicParameters content as Json

1.6.2
* Add feature to Predicates static class to create ISort for a property name without using expressions.
* Fix issue with Predicates' group method to actually set a "real" IList that supports adding further entries.

1.6.1
* Hotfix to replace false binary in package

1.6.0
* Remove all flavors and features except SQL Server and attribute-based mapping
* Create highly restricted version with limited surfaced API (most implementation hidden as internal)

1.5.0
* Update to .NET 4.5
* Update dependencies to latest versions
* Add attribute based mapper and make it default
* Fix minor bugs

Pre-1.5
* See original change log at the original authors' repositories: [Thad Smith and Page Brooks](https://github.com/tmsmith/Dapper-Extensions), [Jörg Battermann](https://github.com/jbattermann/Dapper-Extensions)
