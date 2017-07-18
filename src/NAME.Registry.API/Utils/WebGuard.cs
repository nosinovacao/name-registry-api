using Microsoft.AspNetCore.Mvc;
using NAME.Registry.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAME.DummyRegistryService.Util
{
    /// <summary>
    /// Provides util Guard methods for Asp.Net
    /// </summary>
    public static class WebGuard
    {
        /// <summary>
        /// Executes the action and guards it against an <see cref="EntityNotFoundException" />.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="action">The action.</param>
        /// <param name="successMethod">The success method.</param>
        /// <returns>
        /// Returns <see cref="NotFoundResult" /> if the action fails with an <see cref="EntityNotFoundException" />. Otherwise returns <see cref="OkResult" />.
        /// </returns>
        public static IActionResult GuardNotFoundException(this Controller controller, Action action, Func<Controller, IActionResult> successMethod = null)
        {
            try
            {
                action();
            }
            catch (EntityNotFoundException)
            {
                return controller.NotFound();
            }

            if (successMethod != null)
                return successMethod(controller);

            return controller.Ok();
        }

        /// <summary>
        /// Executes the action and guards it against an <see cref="EntityNotFoundException" />.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="controller">The controller.</param>
        /// <param name="action">The action.</param>
        /// <param name="successMethod">The success method.</param>
        /// <returns>
        /// Returns <see cref="NotFoundResult" /> if the action fails with an <see cref="EntityNotFoundException" />. Otherwise returns <see cref="OkResult" />.
        /// </returns>
        public static IActionResult GuardNotFoundException<TReturn>(this Controller controller, Func<TReturn> action, Func<Controller, TReturn, IActionResult> successMethod = null)
        {
            TReturn result;
            try
            {
                result = action();
            }
            catch (EntityNotFoundException)
            {
                return controller.NotFound();
            }

            if (successMethod != null)
                return successMethod(controller, result);

            return controller.Ok(result);
        }
    }
}