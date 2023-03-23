using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Execution.Instrumentation;
using HotChocolate.Resolvers;
using Microsoft.Extensions.Logging;

namespace StarterTemplate.GraphQL.Logging
{
    public class ConsoleQueryLogger : DiagnosticEventListener
    {
        private static Stopwatch _queryTimer;
        private readonly ILogger<ConsoleQueryLogger> _logger;

        public ConsoleQueryLogger(ILogger<ConsoleQueryLogger> logger)
        {
            _logger = logger;
        }

        public override IActivityScope ExecuteRequest(IRequestContext context)
        {
            return new RequestScope(_logger, context);
        }

        public override void RequestError(IRequestContext context, Exception exception)
        {
            base.RequestError(context, exception);

            _logger.LogInformation(exception, "GraphQL Request Error");
        }

        public override void ResolverError(IMiddlewareContext context, IError error)
        {
            base.ResolverError(context, error);

            _logger.LogInformation(error.Exception, "GraphQL Resolver Error: {code} {message}", error.Code, error.Message);
        }

        public override void SyntaxError(IRequestContext context, IError error)
        {
            base.SyntaxError(context, error);

            _logger.LogInformation(error.Exception, "GraphQL Syntax Error: {code} {message}", error.Code, error.Message);
        }

        public override void TaskError(IExecutionTask task, IError error)
        {
            base.TaskError(task, error);

            _logger.LogInformation(error.Exception, "GraphQL Request Error: {code} {message}", error.Code, error.Message);
        }

        public override void ValidationErrors(IRequestContext context, IReadOnlyList<IError> errors)
        {
            base.ValidationErrors(context, errors);

            _logger.LogInformation("GraphQL Validation Errors: {0}", string.Join(Environment.NewLine, errors.Select(x => x.ToString())));
        }

        private class RequestScope : IActivityScope
        {
            private readonly IRequestContext _context;
            private readonly ILogger<ConsoleQueryLogger> _logger;

            public RequestScope
                (ILogger<ConsoleQueryLogger> logger,
                     IRequestContext context)
            {
                _logger = logger;
                _context = context;
                _queryTimer = new Stopwatch();
                _queryTimer.Start();
            }

            public void Dispose()
            {
                if (_context.Document == null) return;
                StringBuilder stringBuilder =
                    new StringBuilder(_context.Document.ToString(true));
                stringBuilder.AppendLine();
                if (_context.Variables != null)
                {
                    var variablesConcrete =
                        _context.Variables!.ToList();
                    if (variablesConcrete.Count > 0)
                    {
                        stringBuilder.
                            AppendFormat($"Variables {Environment.NewLine}");
                        try
                        {
                            foreach (var variableValue in _context.Variables!)
                            {
                                string PadRightHelper
                                    (string existingString, int lengthToPadTo)
                                {
                                    if (string.IsNullOrEmpty(existingString))
                                        return "".PadRight(lengthToPadTo);
                                    if (existingString.Length > lengthToPadTo)
                                        return existingString.Substring(0, lengthToPadTo);
                                    return existingString + " ".PadRight(lengthToPadTo - existingString.Length);
                                }
                                stringBuilder.AppendFormat(
                                    $"  {PadRightHelper(variableValue.Name, 20)} :  {PadRightHelper(variableValue.Value.ToString(), 20)}: {variableValue.Type}");
                                stringBuilder.AppendFormat($"{Environment.NewLine}");
                            }
                        }
                        catch
                        {
                            // all input type records will land here.
                            stringBuilder.Append("  Formatting Variables Error. Continuing...");
                            stringBuilder.AppendFormat($"{Environment.NewLine}");
                        }
                    }
                }
                _queryTimer.Stop();
                stringBuilder.AppendFormat(
                    $"Ellapsed time for query is {_queryTimer.Elapsed.TotalMilliseconds:0.#} milliseconds.");
                _logger.LogInformation(stringBuilder.ToString());
            }
        }
    }
}