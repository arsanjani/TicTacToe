import Swal from 'sweetalert2';
import type { SweetAlertIcon, SweetAlertResult } from 'sweetalert2';

export interface ConfirmationOptions {
  title: string;
  text?: string;
  icon?: SweetAlertIcon;
  confirmButtonText?: string;
  cancelButtonText?: string;
  theme?: 'default' | 'success' | 'info' | 'dark';
  dangerousAction?: boolean;
}

/**
 * Shows a confirmation dialog with consistent styling
 * @param options Configuration options for the confirmation dialog
 * @returns Promise that resolves to the user's choice
 */
export const showConfirmationDialog = async (
  options: ConfirmationOptions
): Promise<SweetAlertResult<boolean>> => {
  const {
    title,
    text = '',
    icon = 'warning',
    confirmButtonText = 'Confirm',
    cancelButtonText = 'Cancel',
    theme = 'default',
    dangerousAction = false
  } = options;

  // Determine custom class based on theme
  let customClass: string = '';
  switch (theme) {
    case 'success':
      customClass = 'swal-success-theme';
      break;
    case 'info':
      customClass = 'swal-info-theme';
      break;
    case 'dark':
      customClass = 'swal-dark-theme';
      break;
    default:
      customClass = '';
  }

  // Set button colors based on action type
  const confirmButtonColor = dangerousAction ? '#e74c3c' : '#27ae60';
  const cancelButtonColor = '#6c757d';

  return await Swal.fire({
    title,
    text,
    icon,
    showCancelButton: true,
    confirmButtonColor,
    cancelButtonColor,
    confirmButtonText,
    cancelButtonText,
    customClass: {
      popup: `swal-popup ${customClass}`.trim(),
      title: 'swal-title',
      confirmButton: 'swal-confirm-button',
      cancelButton: 'swal-cancel-button'
    },
    buttonsStyling: false, // Use our custom CSS instead of default styling
    allowOutsideClick: false,
    allowEscapeKey: true,
    focusConfirm: false, // Focus cancel button by default for safety
    reverseButtons: true // Cancel on left, confirm on right
  });
};

/**
 * Shows a simple confirmation dialog for dangerous actions (like leaving a game)
 * @param title The title of the confirmation
 * @param text Optional description text
 * @returns Promise that resolves to true if confirmed, false if cancelled
 */
export const showDangerConfirmation = async (
  title: string,
  text?: string
): Promise<boolean> => {
  const result = await showConfirmationDialog({
    title,
    text,
    icon: 'warning',
    confirmButtonText: 'Yes, proceed',
    cancelButtonText: 'Cancel',
    dangerousAction: true
  });

  return result.isConfirmed;
};

/**
 * Shows a success confirmation dialog
 * @param title The title of the confirmation
 * @param text Optional description text
 * @returns Promise that resolves to true if confirmed, false if cancelled
 */
export const showSuccessConfirmation = async (
  title: string,
  text?: string
): Promise<boolean> => {
  const result = await showConfirmationDialog({
    title,
    text,
    icon: 'success',
    theme: 'success',
    confirmButtonText: 'Continue',
    cancelButtonText: 'Cancel'
  });

  return result.isConfirmed;
};

/**
 * Shows an info confirmation dialog
 * @param title The title of the confirmation
 * @param text Optional description text
 * @returns Promise that resolves to true if confirmed, false if cancelled
 */
export const showInfoConfirmation = async (
  title: string,
  text?: string
): Promise<boolean> => {
  const result = await showConfirmationDialog({
    title,
    text,
    icon: 'info',
    theme: 'info',
    confirmButtonText: 'OK',
    cancelButtonText: 'Cancel'
  });

  return result.isConfirmed;
};

/**
 * Shows a simple alert (no cancel option)
 * @param title The title of the alert
 * @param text Optional description text
 * @param icon The icon to display
 * @returns Promise that resolves when dismissed
 */
export const showAlert = async (
  title: string,
  text?: string,
  icon: SweetAlertIcon = 'info'
): Promise<void> => {
  await Swal.fire({
    title,
    text,
    icon,
    confirmButtonText: 'OK',
    customClass: {
      popup: 'swal-popup',
      title: 'swal-title',
      confirmButton: 'swal-confirm-button'
    },
    buttonsStyling: false
  });
};