const paths = {
  dashboard: 'M4 4h7v7H4zM13 4h7v4h-7zM13 11h7v9h-7zM4 14h7v6H4z',
  store: 'M4 9l1.5-5h13L20 9M4 9v11h16V9M4 9h16M9.5 20v-5h5v5',
  box: 'M3 7.5 12 3l9 4.5v9L12 21l-9-4.5zM3 7.5l9 4.5m0 9v-9m9-4.5-9 4.5',
  tag: 'M3 12V4h8l10 10-8 8zM7.5 8.5h.01',
  layers: 'M12 3l9 5-9 5-9-5zM3 12.5l9 5 9-5M3 16.5l9 5 9-5',
  collection: 'M5 4h11v14H5zM8 21h11V7',
  truck: 'M2 6h11v10H2zM13 9h4l4 3v4h-8M6.5 19a1.5 1.5 0 1 0 0-3 1.5 1.5 0 0 0 0 3M17.5 19a1.5 1.5 0 1 0 0-3 1.5 1.5 0 0 0 0 3',
  warehouse: 'M3 10l9-6 9 6v10H3zM8 20v-6h8v6',
  transfer: 'M7 4 3 8l4 4M3 8h14M17 20l4-4-4-4M21 16H7',
  receipt: 'M6 3h12v18l-3-2-3 2-3-2-3 2zM9 8h6M9 12h6',
  list: 'M8 6h13M8 12h13M8 18h13M3 6h.01M3 12h.01M3 18h.01',
  plus: 'M12 5v14M5 12h14',
  edit: 'M4 20h4L19 9l-4-4L4 16zM13.5 6.5l4 4',
  trash: 'M4 7h16M9 7V4h6v3M6 7l1 13h10l1-13',
  close: 'M6 6l12 12M18 6 6 18',
  search: 'M11 4a7 7 0 1 0 0 14 7 7 0 0 0 0-14zM21 21l-4.5-4.5',
  refresh: 'M20 11a8 8 0 0 0-14.5-3.5M4 4v4h4M4 13a8 8 0 0 0 14.5 3.5M20 20v-4h-4',
  menu: 'M4 6h16M4 12h16M4 18h16',
  logout: 'M10 4H5v16h5M15 8l4 4-4 4M19 12H9',
  alert: 'M12 4 2.5 20h19zM12 10v5M12 17.5h.01',
  check: 'M5 12.5l4.5 4.5L19 7.5',
  cart: 'M3 4h2.5l2 11h10.5l2-8H6.5M9.5 20a1 1 0 1 0 0-2 1 1 0 0 0 0 2M17 20a1 1 0 1 0 0-2 1 1 0 0 0 0 2',
  minus: 'M5 12h14',
  image:'M4 5h16v14H4zM4 16l5-5 4 4 3-3 4 4M9 9.5h.01',
}

export function Icon({ name, size = 18, className = '' }) {
  return (
    <svg
      className={`icon ${className}`.trim()}
      width={size}
      height={size}
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="1.6"
      strokeLinecap="round"
      strokeLinejoin="round"
      aria-hidden="true"
    >
      <path d={paths[name] ?? paths.list} />
    </svg>
  )
}
