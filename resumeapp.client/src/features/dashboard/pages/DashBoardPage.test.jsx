import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import {MemoryRouter,Route,Routes,} from 'react-router-dom'
import {beforeEach,describe,expect,it,} from 'vitest'
import DashBoardPage from './DashBoardPage'

function renderDashboard() {
    return render(
        <MemoryRouter initialEntries={['/dashboard']}>
            <Routes>
                <Route path="/dashboard" element={<DashBoardPage />} />
                <Route path="/login" element={<p>Login page</p>} />
                <Route path="/my-resume" element={<p>Resume editor page</p>} />
                <Route
                    path="/my-resume/preview"
                    element={<p>Resume preview page</p>}
                />
                <Route path="/resume" element={<p>Public resume page</p>} />
                <Route path="/" element={<p>Homepage</p>} />
            </Routes>
        </MemoryRouter>,
    )
}

describe('DashBoardPage', () => {
    beforeEach(() => {
        localStorage.clear()
    })

    // Verifies the dashboard content, user details, navigation, and logout behaviour.
    it('renders the dashboard status and action sections', () => {
        renderDashboard()

        expect(
            screen.getByText('Resume Dashboard'),
        ).toBeInTheDocument()
        expect(
            screen.getByRole('heading', {
                name: 'What would you like to do?',
            }),
        ).toBeInTheDocument()
        expect(screen.getByText('Ready to update')).toBeInTheDocument()
        expect(screen.getByText('Authenticated')).toBeInTheDocument()
        expect(screen.getByText('Available')).toBeInTheDocument()
        expect(
            screen.getByRole('button', { name: 'Open Resume Editor' }),
        ).toBeInTheDocument()
        expect(
            screen.getByRole('button', { name: 'Preview My Resume' }),
        ).toBeInTheDocument()
        expect(
            screen.getByRole('button', { name: 'Open Public Page' }),
        ).toBeInTheDocument()
        expect(
            screen.getByRole('button', { name: 'Go to Homepage' }),
        ).toBeInTheDocument()
    })

    it('displays the saved user name, email, and initial', () => {
        localStorage.setItem('user_name', 'Test User')
        localStorage.setItem('user_email', 'user@example.com')

        renderDashboard()

        expect(
            screen.getByRole('heading', {
                name: 'Welcome back, Test User',
            }),
        ).toBeInTheDocument()
        expect(
            screen.getByText('Signed in as user@example.com'),
        ).toBeInTheDocument()
        expect(screen.getByText('T')).toBeInTheDocument()
    })

    it('uses the email initial when the user name is unavailable', () => {
        localStorage.setItem('user_email', 'user@example.com')

        renderDashboard()

        expect(
            screen.getByRole('heading', { name: 'Welcome back' }),
        ).toBeInTheDocument()
        expect(screen.getByText('U')).toBeInTheDocument()
        expect(
            screen.getByText('Signed in as user@example.com'),
        ).toBeInTheDocument()
    })

    it('uses the default user initial when no user details are saved', () => {
        renderDashboard()

        expect(
            screen.getByRole('heading', { name: 'Welcome back' }),
        ).toBeInTheDocument()
        expect(screen.getByText('R')).toBeInTheDocument()
        expect(
            screen.queryByText(/Signed in as/i),
        ).not.toBeInTheDocument()
    })

    it('does not display signed-in email text when only a name is saved', () => {
        localStorage.setItem('user_name', 'Test User')

        renderDashboard()

        expect(
            screen.getByRole('heading', {
                name: 'Welcome back, Test User',
            }),
        ).toBeInTheDocument()
        expect(
            screen.queryByText(/Signed in as/i),
        ).not.toBeInTheDocument()
    })

    it('opens the resume editor', async () => {
        const user = userEvent.setup()
        renderDashboard()

        await user.click(
            screen.getByRole('button', { name: 'Open Resume Editor' }),
        )

        expect(
            screen.getByText('Resume editor page'),
        ).toBeInTheDocument()
    })

    it('opens the personal resume preview', async () => {
        const user = userEvent.setup()
        renderDashboard()

        await user.click(
            screen.getByRole('button', { name: 'Preview My Resume' }),
        )

        expect(
            screen.getByText('Resume preview page'),
        ).toBeInTheDocument()
    })

    it('opens the public resume page', async () => {
        const user = userEvent.setup()
        renderDashboard()

        await user.click(
            screen.getByRole('button', { name: 'Open Public Page' }),
        )

        expect(
            screen.getByText('Public resume page'),
        ).toBeInTheDocument()
    })

    it('returns to the homepage', async () => {
        const user = userEvent.setup()
        renderDashboard()

        await user.click(
            screen.getByRole('button', { name: 'Go to Homepage' }),
        )

        expect(screen.getByText('Homepage')).toBeInTheDocument()
    })

    it('clears authentication data and redirects to login on logout', async () => {
        localStorage.setItem('resume_app_token', 'test-token')
        localStorage.setItem('user_email', 'user@example.com')
        localStorage.setItem('user_name', 'Test User')
        localStorage.setItem('unrelated_setting', 'keep-me')

        const user = userEvent.setup()
        renderDashboard()

        await user.click(
            screen.getByRole('button', { name: 'Logout' }),
        )

        expect(localStorage.getItem('resume_app_token')).toBeNull()
        expect(localStorage.getItem('user_email')).toBeNull()
        expect(localStorage.getItem('user_name')).toBeNull()
        expect(localStorage.getItem('unrelated_setting')).toBe('keep-me')
        expect(screen.getByText('Login page')).toBeInTheDocument()
    })


})