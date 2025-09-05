import React from 'react';
import { Menu, Transition } from '@headlessui/react';
import { 
  Bars3Icon, 
  UserCircleIcon, 
  ArrowRightOnRectangleIcon 
} from '@heroicons/react/24/outline';
import { useAuthStore } from '../../stores/auth';
import { Button } from '../ui';
import { cn } from '../../utils/cn';

interface HeaderProps {
  onToggleSidebar: () => void;
}

export function Header({ onToggleSidebar }: HeaderProps) {
  const { user, logout } = useAuthStore();

  const handleLogout = async () => {
    await logout();
  };

  return (
    <header className="bg-white border-b border-secondary-200 h-16">
      <div className="flex items-center justify-between h-full px-4">
        <div className="flex items-center">
          <Button
            variant="ghost"
            size="sm"
            onClick={onToggleSidebar}
            className="p-2"
          >
            <Bars3Icon className="h-5 w-5" />
          </Button>
        </div>

        <div className="flex items-center space-x-4">
          {/* User Menu */}
          <Menu as="div" className="relative">
            <Menu.Button className="flex items-center space-x-2 p-2 rounded-md hover:bg-secondary-100">
              <UserCircleIcon className="h-6 w-6 text-secondary-600" />
              <div className="text-left">
                <p className="text-sm font-medium text-secondary-900">
                  {user?.nome}
                </p>
                <p className="text-xs text-secondary-500">
                  {user?.empresaNome}
                </p>
              </div>
            </Menu.Button>

            <Transition
              as={React.Fragment}
              enter="transition ease-out duration-100"
              enterFrom="transform opacity-0 scale-95"
              enterTo="transform opacity-100 scale-100"
              leave="transition ease-in duration-75"
              leaveFrom="transform opacity-100 scale-100"
              leaveTo="transform opacity-0 scale-95"
            >
              <Menu.Items className="absolute right-0 mt-2 w-48 origin-top-right rounded-md bg-white py-1 shadow-lg ring-1 ring-black ring-opacity-5 focus:outline-none">
                <Menu.Item>
                  {({ active }) => (
                    <button
                      onClick={handleLogout}
                      className={cn(
                        'flex w-full items-center px-4 py-2 text-sm',
                        active ? 'bg-secondary-100 text-secondary-900' : 'text-secondary-700'
                      )}
                    >
                      <ArrowRightOnRectangleIcon className="mr-3 h-4 w-4" />
                      Sair
                    </button>
                  )}
                </Menu.Item>
              </Menu.Items>
            </Transition>
          </Menu>
        </div>
      </div>
    </header>
  );
}