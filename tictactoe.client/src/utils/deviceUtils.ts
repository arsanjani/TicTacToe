/**
 * Utility functions for device detection and browser capability checks
 */

/**
 * Detects if the current device is likely a mobile device
 * Uses multiple methods for more accurate detection
 */
export const isMobileDevice = (): boolean => {
  // Check user agent for mobile indicators
  const userAgent = navigator.userAgent || navigator.vendor || (window as Window & { opera?: string }).opera || '';
  const mobileRegex = /android|webos|iphone|ipad|ipod|blackberry|iemobile|opera mini/i;
  
  // Check if user agent contains mobile indicators
  const hasUserAgentMobile = mobileRegex.test(userAgent);
  
  // Check for touch support (not foolproof as some desktops have touch)
  const hasTouchSupport = 'ontouchstart' in window || navigator.maxTouchPoints > 0;
  
  // Check screen size (typical mobile breakpoint)
  const hasSmallScreen = window.innerWidth <= 768;
  
  // Check for mobile-specific CSS media query support
  const hasCoarsePointer = window.matchMedia('(pointer: coarse)').matches;
  
  // Combine multiple indicators for better accuracy
  // If user agent indicates mobile OR (has touch + small screen + coarse pointer)
  return hasUserAgentMobile || (hasTouchSupport && hasSmallScreen && hasCoarsePointer);
};

/**
 * Checks if the Web Share API is supported and available
 */
export const isWebShareSupported = (): boolean => {
  return 'share' in navigator && typeof navigator.share === 'function';
};

/**
 * Checks if the device should use the share functionality
 * Combines mobile detection and Web Share API support
 */
export const shouldUseWebShare = (): boolean => {
  return isMobileDevice() && isWebShareSupported();
};

/**
 * Data interface for Web Share API
 */
export interface ShareData {
  title?: string;
  text?: string;
  url?: string;
}

/**
 * Attempts to share content using the Web Share API
 * Falls back to clipboard copy if sharing fails
 */
export const shareContent = async (data: ShareData): Promise<boolean> => {
  if (!isWebShareSupported()) {
    throw new Error('Web Share API not supported');
  }
  
  try {
    await navigator.share(data);
    return true;
  } catch (error) {
    // User cancelled or other error occurred
    console.warn('Web Share failed:', error);
    return false;
  }
};

/**
 * Copies text to clipboard as fallback
 */
export const copyToClipboard = async (text: string): Promise<boolean> => {
  try {
    await navigator.clipboard.writeText(text);
    return true;
  } catch (error) {
    console.error('Failed to copy to clipboard:', error);
    return false;
  }
}; 