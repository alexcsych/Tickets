using System;
using System.Collections.Generic;
using System.Text;
using Tickets.ViewModels;

namespace Tickets.Tests
{
    public class ProfileViewModelTests
    {
        [Fact]
        public void Email_ShouldReturnError_WhenFormatIsInvalid()
        {
            var viewModel = new ProfileViewModel(null!);
            viewModel.GetType().GetField("_isEmailTouched", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(viewModel, true);

            viewModel.Email = "invalid-email";
            string error = viewModel["Email"];

            Assert.Equal("Невірний формат пошти!", error);
        }

        [Fact]
        public void ConfirmPassword_ShouldReturnError_WhenPasswordsDoNotMatch()
        {
            var viewModel = new ProfileViewModel(null!);
            viewModel.GetType().GetField("_isPasswordTouched", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(viewModel, true);
            viewModel.GetType().GetField("_isConfirmPasswordTouched", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(viewModel, true);

            viewModel.Password = "123456";
            viewModel.ConfirmPassword = "654321";
            string error = viewModel["ConfirmPassword"];

            Assert.Equal("Паролі не співпадають!", error);
        }

        [Fact]
        public void Name_ShouldReturnError_WhenTooShort()
        {
            var viewModel = new ProfileViewModel(null!);
            viewModel.GetType().GetField("_isNameTouched", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(viewModel, true);

            viewModel.Name = "A";
            string error = viewModel["Name"];

            Assert.Equal("Мін. 2 символи!", error);
        }
    }
}
