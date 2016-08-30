Logging using log4net
=======================
1. Log4net library is used as the chosen logging library.
2. Log4net has a concept of loggers, log levels, appenders, layouts, filters and contexts.

********************************************************************************
Loggers
********************************************************************************
a. Loggers are the main components that are used by the client applications. It is also the component that generates the log messages.
b. A logger is maintained as a "named entity" inside the log4net framework. That means that we don't need to pass around the Logger instance between different classes or objects to reuse it. Instead, you can call it with the name anywhere in the application.
c. The loggers maintained inside of the log4net framework follow a hierarchical organization similar to namespaces in code.
d. E.g. the logger "SIS.Business" will be an ancestor of the logger "SIS.Business.Model".
e. Each logger inherits properties from its parent.
f. The root logger is the parent of all other names loggers.

********************************************************************************
Log Levels
********************************************************************************
a. Log4net defines the following logging levels, each level operates at a different priority level:
  - OFF (Highest)
  - FATAL
  - ERROR
  - WARN
  - INFO
  - DEBUG
  - ALL (Lowest)
b. Using the configuration, every logger is assigned a particular log level.
c. If a log level is not assigned then the logger inherits the log level of its parent logger.
d. A message is logged only if its log level is greater than equal to the level of the logger.

********************************************************************************
Appenders
********************************************************************************
a. Appenders specify the output medium for the log messages.
b. The appenders append themselves to the loggers and relay the output to one or more output streams.
c. Filters can be attached to appenders.

********************************************************************************
Layouts
********************************************************************************
a. Layouts are used to show the final formatted output to the user.
b. Layouts can be linear of xml.
c. Custom layouts can also be implemented.

********************************************************************************
Context
********************************************************************************
There are various context that you can maintain with the log4net runtime like Global Context, Thread Context and Logical Thread Context.

Global Context
---------------
A global context is shared across all application threads and app domains. If two threads set the same property on the GlobalContext, one value will overwrite the other.

Thread Context
---------------
Any properties set in this context are scoped to the calling thread. In other words, in this context two threads can set the same property to different values without stomping on each other. ThreadContext can store property values in stacks (available via the Stacks static property of each class).  These stacks are very handy, as they allow you to track program states in the context of a log message.

More information about contexts can be read at: http://www.beefycode.com/post/Log4Net-Tutorial-pt-6-Log-Event-Context.aspx

********************************************************************************
Filters
********************************************************************************
Filters can be applied to individual appenders via the log4net configuration, and they help the appender determine whether a log event should be processed by the appender. They may sound like the appender threshold property, and indeed some filters behave similarly; however, filters offer far more control over logging behavior.

Following is a list of the filters available in the default log4net distribution:
a. log4net.Filter.LevelMatchFilter: Filters log events that match a specific logging level; alternatively this can be configured to filter events that DO NOT match a specific logging level.
b. log4net.Filter.LevelRangeFilter: Similar to the LevelMatchFilter, except that instead of filtering a single log level, this filters on an inclusive range of contiguous levels.
c. log4net.Filter.LoggerMatchFilter: Filters log events based on the name of the logger object from which they are emitted.
d. log4net.Filter.StringMatchFilter: Filters log events based on a string or regular expression match against the log message.
e. log4net.Filter.PropertyFilter: Filters log events based on a value or regular expression match against a specific context property.
f. log4net.Filter.DenyAllFilter: Effectively drops all logging events for the appender.

More information about filters can be read at: http://www.beefycode.com/post/Log4Net-Tutorial-pt-7-Filters.aspx

Configuration of log4net
********************************************************************************
a. The SIS application will store the log4net configuration in a separate file (i.e. not within Web.config or App.config)
b. One logger per module will be defined in SIS.
c. Multiple loggers will help to enable or disable the logging of a specific functionality to test/debug/unearth problems in specific parts of the application.
d. A separate config file will be maintained for the debug build and the release build. (using pre-processor directives).
e. The machine 

Using log4net
********************************************************************************
Source: http://www.beefycode.com/post/Log4Net-Recommended-Practices-pt-1-Your-Code.aspx

1. Use a unique logger object for each type in your application. This will auto magically tag each log message with the class that wrote it. Using this technique provides a log that reads like a novel of your application's activity.  

2. Whenever you catch an exception, log it.  Even if you just plan to throw it again, log it. In addition, log4net knows how to format Exception objects, so don't try and build your own string from the exception data.

3. Don't be afraid to pepper your application with piddly little log messages. Each one becomes a marker of activity and can prove invaluable when trying to work an issue. While logging does consume resources, that consumption can be controlled and optimized. In all but the most CPU-intensive applications, the impact of logging won't be noticed when configured properly.

4. Whenever you use one of the xxxFormat overrides, be extra-special-careful about validating the argument list. It's very easy to forget to check for null references before using a property accessor, for example, because "it's just logging code."

5. Always remember that the argument list is evaluated before the logging method is called. If an argument to the log method is expensive to obtain, be sure to guard your log statement with a check of the appropriate IsXxxEnabled property.

6. Before an object calls on a shared component, consider pushing a tag onto the log context stack. This will provide continuity in the log, allowing you to determine the caller of the shared code that logged a particular message.

7. Whenever you use a formatted log statement, surround the format argument placeholders (the {}'s) with brackets or parentheses. Doing this will mark the areas in each log message that vary, making the log scan a bit easier. In addition, it makes empty or null formatting arguments more obvious.