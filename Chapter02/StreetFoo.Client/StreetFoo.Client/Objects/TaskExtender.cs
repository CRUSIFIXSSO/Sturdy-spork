﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreetFoo.Client
{
    public static class TaskExtender
    {
        public static Task ChainFailureHandler(this Task task, FailureHandler failure, Action complete)
        {
            // create a handler that will raise an exception if an operation fails...
            return task.ContinueWith((t) =>
            {
                try
                {
                    // create a fatal error bucket...
                    ErrorBucket fatal = ErrorBucket.CreateFatalBucket(t.Exception);
                    failure(task, fatal);
                }
                finally
                {
                    // complete?
                    complete();
                }

            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        // adds the task to the context...
        public static Task AddToContext(this Task task, CommandExecutionContext context)
        {
            // if we have a context...
            if (context != null)
                context.AddTask(task);

            return task;
        }
    }
}
