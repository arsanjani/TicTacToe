import { useState, useEffect } from 'react';

export const useScrollDirection = () => {
  const [scrollDirection, setScrollDirection] = useState<"up" | "down" | null>(
    null
  );
  const [showSocialIcons, setShowSocialIcons] = useState(false);
  const [lastScrollY, setLastScrollY] = useState(0);

  useEffect(() => {
    const updateScrollDirection = () => {
      const scrollY = window.pageYOffset;
      const direction = scrollY > lastScrollY ? "down" : "up";

      // Check if scroll position has changed significantly
      if (Math.abs(scrollY - lastScrollY) > 10) {
        // If this is the first scroll detection or direction has changed
        if (scrollDirection !== direction) {
          setScrollDirection(direction);
          setShowSocialIcons(direction === "down");
        }
      }
      setLastScrollY(scrollY > 0 ? scrollY : 0);
    };

    // Set initial scroll position
    setLastScrollY(window.pageYOffset);

    window.addEventListener("scroll", updateScrollDirection);
    return () => {
      window.removeEventListener("scroll", updateScrollDirection);
    };
  }, [scrollDirection, lastScrollY]);

  return { scrollDirection, showSocialIcons };
}; 