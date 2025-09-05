import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import { 
  HomeIcon,
  BuildingOfficeIcon,
  BuildingStorefrontIcon,
  FolderIcon,
  ChevronDownIcon,
  ChevronRightIcon,
} from '@heroicons/react/24/outline';
import { cn } from '../../utils/cn';

interface SidebarProps {
  isCollapsed: boolean;
}

interface MenuItemProps {
  icon: React.ElementType;
  label: string;
  href?: string;
  children?: MenuItemProps[];
}

const menuItems: MenuItemProps[] = [
  {
    icon: HomeIcon,
    label: 'Dashboard',
    href: '/dashboard',
  },
  {
    icon: FolderIcon,
    label: 'Cadastros',
    children: [
      {
        icon: BuildingOfficeIcon,
        label: 'Empresas',
        href: '/cadastros/empresas',
      },
      {
        icon: BuildingStorefrontIcon,
        label: 'Filiais',
        href: '/cadastros/filiais',
      },
      {
        icon: FolderIcon,
        label: 'Agrupamentos',
        href: '/cadastros/agrupamentos',
      },
      {
        icon: FolderIcon,
        label: 'Sub Agrupamentos',
        href: '/cadastros/subagrupamentos',
      },
      {
        icon: FolderIcon,
        label: 'Centros de Custo',
        href: '/cadastros/centros-custo',
      },
    ],
  },
];

interface MenuItemComponentProps {
  item: MenuItemProps;
  isCollapsed: boolean;
  level?: number;
}

function MenuItem({ item, isCollapsed, level = 0 }: MenuItemComponentProps) {
  const location = useLocation();
  const [isOpen, setIsOpen] = React.useState(false);

  const hasChildren = item.children && item.children.length > 0;
  const isActive = item.href ? location.pathname === item.href : false;
  const hasActiveChild = hasChildren && item.children?.some(child => location.pathname === child.href);

  React.useEffect(() => {
    if (hasActiveChild) {
      setIsOpen(true);
    }
  }, [hasActiveChild]);

  const content = (
    <div
      className={cn(
        'flex items-center w-full px-3 py-2 text-sm font-medium rounded-md transition-colors',
        'hover:bg-secondary-100 hover:text-secondary-900',
        isActive && 'bg-primary-100 text-primary-900',
        level > 0 && 'pl-8'
      )}
      onClick={hasChildren ? () => setIsOpen(!isOpen) : undefined}
    >
      <item.icon className="h-5 w-5 flex-shrink-0" />
      {!isCollapsed && (
        <>
          <span className="ml-3 flex-1">{item.label}</span>
          {hasChildren && (
            <ChevronRightIcon
              className={cn(
                'h-4 w-4 transition-transform',
                isOpen && 'rotate-90'
              )}
            />
          )}
        </>
      )}
    </div>
  );

  if (item.href && !hasChildren) {
    return (
      <Link to={item.href}>
        {content}
      </Link>
    );
  }

  return (
    <div>
      <button
        type="button"
        className="w-full text-left"
        disabled={isCollapsed && hasChildren}
      >
        {content}
      </button>
      {hasChildren && !isCollapsed && isOpen && (
        <div className="mt-1 space-y-1">
          {item.children?.map((child, index) => (
            <MenuItem
              key={index}
              item={child}
              isCollapsed={isCollapsed}
              level={level + 1}
            />
          ))}
        </div>
      )}
    </div>
  );
}

export function Sidebar({ isCollapsed }: SidebarProps) {
  return (
    <div
      className={cn(
        'bg-white border-r border-secondary-200 transition-all duration-300',
        isCollapsed ? 'w-16' : 'w-64'
      )}
    >
      <div className="flex flex-col h-full">
        <div className="flex items-center h-16 px-4 border-b border-secondary-200">
          <div className="flex items-center">
            <div className="w-8 h-8 bg-primary-600 rounded-md flex items-center justify-center">
              <span className="text-white font-bold text-sm">ERP</span>
            </div>
            {!isCollapsed && (
              <div className="ml-3">
                <p className="text-sm font-medium text-secondary-900">
                  Sistema ERP
                </p>
                <p className="text-xs text-secondary-500">
                  Restaurantes
                </p>
              </div>
            )}
          </div>
        </div>

        <nav className="flex-1 px-3 py-4 space-y-1 overflow-y-auto">
          {menuItems.map((item, index) => (
            <MenuItem
              key={index}
              item={item}
              isCollapsed={isCollapsed}
            />
          ))}
        </nav>
      </div>
    </div>
  );
}