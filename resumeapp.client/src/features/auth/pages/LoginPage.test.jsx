import { render, screen, fireEvent } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import {MemoryRouter,Route,Routes,} from 'react-router-dom'
import {afterEach,beforeEach,describe,expect,it,vi,} from 'vitest'
import axiosClient from '../../../core/api/axiosClient'
import LoginPage from './LoginPage'

vi.mock('../../../core/api/axiosClient', () => ({
    default: {
        post: vi.fn(),
    },
}))

function renderLoginPage() {
    return render(
        <MemoryRouter initialEntries={['/login']}>
            <Routes>
                <Route path="/login" element={<LoginPage />} />
                <Route path="/dashboard" element={<p>Dashboard page</p>} />
                <Route path="/register" element={<p>Register page</p>} />
            </Routes>
        </MemoryRouter>,
    )
}

describe('LoginPage', () => {
    let consoleErrorSpy

    beforeEach(() => {
        localStorage.clear()
        vi.clearAllMocks()
        consoleErrorSpy = vi
            .spyOn(console, 'error')
            .mockImplementation(() => { })
    })

    afterEach(() => {
        consoleErrorSpy.mockRestore()
    })

    // Verifies that the login form displays all required controls and links.
    it('renders the complete login form', () => {
        renderLoginPage()

        expect(
            screen.getByRole('heading', { name: 'Login' }),
        ).toBeInTheDocument()
        expect(
            screen.getByLabelText('Email address'),
        ).toBeInTheDocument()
        expect(screen.getByLabelText('Password')).toBeInTheDocument()
        expect(
            screen.getByRole('button', { name: 'Login' }),
        ).toBeInTheDocument()
        expect(
            screen.getByRole('link', { name: 'Register' }),
        ).toHaveAttribute('href', '/register')
    })

    it('updates the form fields when the user types', () => {
        const testEmail = ['user', 'example.com'].join('@')

        renderLoginPage()

        const emailInput = screen.getByLabelText('Email address')
        const passwordInput = screen.getByLabelText('Password')

        fireEvent.change(emailInput, {
            target: { value: testEmail },
        })
        fireEvent.change(passwordInput, {
            target: { value: 'Password123!' },
        })

        expect(emailInput).toHaveValue(testEmail)
        expect(passwordInput).toHaveValue('Password123!')
    })

    // Verifies that the browser prevents submission when required fields are empty.
    it('does not submit the form when required fields are empty', async () => {
        const user = userEvent.setup()
        renderLoginPage()

        await user.click(screen.getByRole('button', { name: 'Login' }))

        expect(axiosClient.post).not.toHaveBeenCalled()
    })

    // Updated: uses deterministic events to avoid slow typing and test interference.
    it('submits the entered credentials to the API', () => {
        const testEmail = ['user', 'example.com'].join('@')

        axiosClient.post.mockReturnValue(new Promise(() => { }))

        renderLoginPage()

        fireEvent.change(screen.getByLabelText('Email address'), {
            target: { value: testEmail },
        })
        fireEvent.change(screen.getByLabelText('Password'), {
            target: { value: 'Password123!' },
        })

        const form = screen
            .getByRole('button', { name: 'Login' })
            .closest('form')

        fireEvent.submit(form)

        expect(axiosClient.post).toHaveBeenCalledWith('/auth/login', {
            email: testEmail,
            password: 'Password123!',
        })
    })

    // Verifies that successful login saves user data and opens the dashboard.
    it('stores authentication data and redirects after successful login', async () => {
        axiosClient.post.mockResolvedValue({
            data: {
                token: 'test-token',
                email: 'user@example.com',
                fullName: 'Test User',
            },
        })

        const user = userEvent.setup()
        renderLoginPage()

        await user.type(
            screen.getByLabelText('Email address'),
            'user@example.com',
        )
        await user.type(
            screen.getByLabelText('Password'),
            'Password123!',
        )
        await user.click(screen.getByRole('button', { name: 'Login' }))

        expect(
            await screen.findByText('Dashboard page'),
        ).toBeInTheDocument()
        expect(localStorage.getItem('resume_app_token')).toBe('test-token')
        expect(localStorage.getItem('user_email')).toBe('user@example.com')
        expect(localStorage.getItem('user_name')).toBe('Test User')
    })

    // Verifies that the submit button displays its loading state during login.
    it('disables the submit button while the login request is running', async () => {
        axiosClient.post.mockReturnValue(new Promise(() => { }))

        const user = userEvent.setup()
        renderLoginPage()

        await user.type(
            screen.getByLabelText('Email address'),
            'user@example.com',
        )
        await user.type(
            screen.getByLabelText('Password'),
            'Password123!',
        )
        await user.click(screen.getByRole('button', { name: 'Login' }))

        expect(
            screen.getByRole('button', { name: 'Logging in...' }),
        ).toBeDisabled()
    })

    // Verifies that an error message returned by the API is displayed.
    it('displays the API error when login fails', async () => {
        axiosClient.post.mockRejectedValue({
            response: {
                data: {
                    message: 'Invalid login credentials.',
                },
            },
        })

        const user = userEvent.setup()
        renderLoginPage()

        await user.type(
            screen.getByLabelText('Email address'),
            'wrong@example.com',
        )
        await user.type(
            screen.getByLabelText('Password'),
            'WrongPassword',
        )
        await user.click(screen.getByRole('button', { name: 'Login' }))

        expect(
            await screen.findByText('Invalid login credentials.'),
        ).toBeInTheDocument()
        expect(localStorage.getItem('resume_app_token')).toBeNull()
    })

    // Verifies that authenticated users are redirected away from the login page.
    it('redirects an already logged-in user to the dashboard', async () => {
        localStorage.setItem('resume_app_token', 'existing-token')

        renderLoginPage()

        expect(
            await screen.findByText('Dashboard page'),
        ).toBeInTheDocument()
        expect(axiosClient.post).not.toHaveBeenCalled()
    })


})