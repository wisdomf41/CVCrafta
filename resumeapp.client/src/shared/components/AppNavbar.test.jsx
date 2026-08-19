import { render, screen } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { describe, expect, it } from 'vitest'
import AppNavbar from './AppNavbar'
import userEvent from '@testing-library/user-event'
import {Route,Routes,} from 'react-router-dom'

describe('AppNavbar', () => {
    // Verifies that visitors see public navigation and authentication links.
    it('renders the correct links when the user is logged out', () => {
        localStorage.clear()

        render(
            <MemoryRouter initialEntries={['/']}>
                <AppNavbar />
            </MemoryRouter>,
        )

        expect(screen.getByRole('navigation')).toBeInTheDocument()
        expect(screen.getByRole('link', { name: 'Home' })).toBeInTheDocument()
        expect(
            screen.getByRole('link', { name: 'Public Resume' }),
        ).toBeInTheDocument()
        expect(screen.getByRole('link', { name: 'Login' })).toBeInTheDocument()
        expect(screen.getByRole('link', { name: 'Register' })).toBeInTheDocument()

        expect(
            screen.queryByRole('link', { name: 'Dashboard' }),
        ).not.toBeInTheDocument()
        expect(
            screen.queryByRole('button', { name: 'Logout' }),
        ).not.toBeInTheDocument()
    })

    // Verifies that authenticated users see their private navigation links and logout button.
    it('renders the correct links when the user is logged in', () => {
        localStorage.clear()
        localStorage.setItem('resume_app_token', 'test-token')

        render(
            <MemoryRouter initialEntries={['/dashboard']}>
                <AppNavbar />
            </MemoryRouter>,
        )

        expect(
            screen.getByRole('link', { name: 'Dashboard' }),
        ).toBeInTheDocument()
        expect(
            screen.getByRole('link', { name: 'My Resume' }),
        ).toBeInTheDocument()
        expect(screen.getByRole('link', { name: 'Preview' })).toBeInTheDocument()
        expect(
            screen.getByRole('button', { name: 'Logout' }),
        ).toBeInTheDocument()

        expect(
            screen.queryByRole('link', { name: 'Login' }),
        ).not.toBeInTheDocument()
        expect(
            screen.queryByRole('link', { name: 'Register' }),
        ).not.toBeInTheDocument()
    })

    // Verifies that the mobile navigation menu opens and closes when its toggle button is clicked.
    it('opens and closes the mobile navigation menu', async () => {
        localStorage.clear()
        const user = userEvent.setup()

        render(
            <MemoryRouter initialEntries={['/']}>
                <AppNavbar />
            </MemoryRouter>,
        )

        const menuButton = screen.getByRole('button', {
            name: 'Toggle navigation menu',
        })

        expect(menuButton).toHaveAttribute('aria-expanded', 'false')
        expect(
            screen.queryByRole('link', { name: 'Create Account' }),
        ).not.toBeInTheDocument()

        await user.click(menuButton)

        expect(menuButton).toHaveAttribute('aria-expanded', 'true')
        expect(
            screen.getByRole('link', { name: 'Create Account' }),
        ).toBeInTheDocument()

        await user.click(menuButton)

        expect(menuButton).toHaveAttribute('aria-expanded', 'false')
        expect(
            screen.queryByRole('link', { name: 'Create Account' }),
        ).not.toBeInTheDocument()
    })

    // Verifies that logout clears the user's saved authentication data and redirects to login.
    it('logs the user out and redirects to the login page', async () => {
        localStorage.clear()
        localStorage.setItem('resume_app_token', 'test-token')
        localStorage.setItem('user_email', 'user@example.com')
        localStorage.setItem('user_name', 'Test User')

        const user = userEvent.setup()

        render(
            <MemoryRouter initialEntries={['/dashboard']}>
                <AppNavbar />

                <Routes>
                    <Route path="/dashboard" element={<p>Dashboard page</p>} />
                    <Route path="/login" element={<p>Login page</p>} />
                </Routes>
            </MemoryRouter>,
        )

        await user.click(screen.getByRole('button', { name: 'Logout' }))

        expect(localStorage.getItem('resume_app_token')).toBeNull()
        expect(localStorage.getItem('user_email')).toBeNull()
        expect(localStorage.getItem('user_name')).toBeNull()
        expect(screen.getByText('Login page')).toBeInTheDocument()
    })

    // Verifies that selecting a mobile navigation link closes the menu.
    it('closes the mobile navigation menu when a link is selected', async () => {
        localStorage.clear()
        const user = userEvent.setup()

        render(
            <MemoryRouter initialEntries={['/']}>
                <AppNavbar />
            </MemoryRouter>,
        )

        const menuButton = screen.getByRole('button', {
            name: 'Toggle navigation menu',
        })

        await user.click(menuButton)

        expect(menuButton).toHaveAttribute('aria-expanded', 'true')

        await user.click(
            screen.getByRole('link', { name: 'Create Account' }),
        )

        expect(menuButton).toHaveAttribute('aria-expanded', 'false')
        expect(
            screen.queryByRole('link', { name: 'Create Account' }),
        ).not.toBeInTheDocument()
    })

    // Verifies that the link for the current page receives the active navigation styling.
    it('highlights the link for the current page', () => {
        localStorage.clear()

        render(
            <MemoryRouter initialEntries={['/resume']}>
                <AppNavbar />
            </MemoryRouter>,
        )

        const publicResumeLink = screen.getByRole('link', {
            name: 'Public Resume',
        })

        const homeLink = screen.getByRole('link', {
            name: 'Home',
        })

        expect(publicResumeLink).toHaveClass('bg-slate-950', 'text-white')
        expect(homeLink).not.toHaveClass('bg-slate-950', 'text-white')
    })


})
