using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Models;
using RaceDayMVC.Services;
using System.Text.Json;

namespace RaceDayMVC.Controllers;

public class AccountController : Controller
{
    private readonly RaceDayApiClient _api;

    public AccountController(RaceDayApiClient api)
    {
        _api = api;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        using var result = await _api.PostJsonAsync(
            "api/Auth/login",
            new
            {
                model.Email,
                model.Password
            });

        if (result == null)
        {
            ModelState.AddModelError(
                "",
                "Login failed. Please check your details and API connection.");

            return View(model);
        }

        var root = result.RootElement;

        string token = ReadValue(root, "token");

        if (string.IsNullOrWhiteSpace(token))
        {
            ModelState.AddModelError(
                "",
                "The API did not return an authentication token.");

            return View(model);
        }

        string role = ReadValue(root, "role");
        string username = ReadValue(root, "username");

        HttpContext.Session.SetString("Token", token);
        HttpContext.Session.SetString("Role", role);
        HttpContext.Session.SetString("Username", username);

        if (role.Equals("Organiser",
            StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction("Index", "Organiser");
        }

        return RedirectToAction("Index", "Participant");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        bool success = await _api.PostAsync(
            "api/Auth/register",
            new
            {
                model.Username,
                model.Email,
                model.Password,
                model.Role
            });

        if (!success)
        {
            ModelState.AddModelError(
                "",
                "Registration failed. Please check your details.");

            return View(model);
        }

        TempData["Success"] =
            "Registration successful. Please log in.";

        return RedirectToAction("Login");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        return RedirectToAction("Login");
    }

    private static string ReadValue(
        JsonElement element,
        string propertyName)
    {
        foreach (var property in element.EnumerateObject())
        {
            if (property.Name.Equals(
                propertyName,
                StringComparison.OrdinalIgnoreCase))
            {
                return property.Value.ToString();
            }
        }

        return "";
    }
}