import { useState, useEffect } from 'react';

export const useScrollDirection = () => {
  const [scrollDirection, setScrollDirection] = useState<'up' | 'down'>('down');
  const [showSocialIcons, setShowSocialIcons] = useState(false);
  const [lastScrollY, setLastScrollY] = useState(0);

  useEffect(() => {
    const updateScrollDirection = () => {
      const scrollY = window.pageYOffset;
      const direction = scrollY > lastScrollY ? 'down' : 'up';
      
      if (direction !== scrollDirection && (scrollY - lastScrollY > 10 || scrollY - lastScrollY < -10)) {
        setScrollDirection(direction);
        setShowSocialIcons(direction === 'down');
      }
      setLastScrollY(scrollY > 0 ? scrollY : 0);
    };

    window.addEventListener('scroll', updateScrollDirection);
    return () => {
      window.removeEventListener('scroll', updateScrollDirection);
    };
  }, [scrollDirection, lastScrollY]);

  return { scrollDirection, showSocialIcons };
}; 