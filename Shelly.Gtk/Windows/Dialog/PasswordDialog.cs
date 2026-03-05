using Shelly.Gtk.Services;
using Gtk;

namespace Shelly.Gtk.Windows.Dialog;

public class PasswordDialog(ICredentialManager credentialManager)
{
    public void ShowPasswordDialog(string reason)
    {
        var dialog = Window.New();
        dialog.SetTitle("Authentication Required");
        dialog.SetModal(true);
        dialog.SetDefaultSize(400, 200);
        dialog.SetIconName("shelly");

        var box = Box.New(Orientation.Vertical, 12);
        box.SetMarginTop(20);
        box.SetMarginBottom(20);
        box.SetMarginStart(20);
        box.SetMarginEnd(20);

        var label = Label.New($"Password needed to execute: {reason}.");

        box.Append(label);

        var errorLabel = Label.New("");

        var passwordEntry = PasswordEntry.New();
        passwordEntry.SetShowPeekIcon(true);
        box.Append(passwordEntry);
        box.Append(errorLabel);

        var buttonBox = Box.New(Orientation.Horizontal, 8);
        buttonBox.SetHalign(Align.End);

        var cancelButton = Button.NewWithLabel("Cancel");
        var submitButton = Button.NewWithLabel("Authenticate");

        cancelButton.OnClicked += async (s, e) =>
        {
            await credentialManager.CompleteCredentialRequestAsync(false);
            dialog.Close();
        };

        submitButton.OnClicked += async (s, e) =>
        {
            var password = passwordEntry.GetText();
            credentialManager.StorePassword(password);
            await credentialManager.CompleteCredentialRequestAsync(true);

            if (credentialManager.IsValidated)
            {
                dialog.Close();
            }
            else
            {
                errorLabel.SetText("Incorrect password. Try again.");
                passwordEntry.SetText("");
            }
        };

        // Allow Enter key to submit
        passwordEntry.OnActivate += (s, e) => submitButton.Activate();

        buttonBox.Append(cancelButton);
        buttonBox.Append(submitButton);
        box.Append(buttonBox);

        dialog.SetChild(box);
        dialog.Show();
    }
}