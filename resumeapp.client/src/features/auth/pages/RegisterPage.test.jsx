import {act,fireEvent,render,screen,} from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import {MemoryRouter,Route,Routes,} from 'react-router-dom'
import {afterEach,beforeEach,describe,expect,it,vi,} from 'vitest'
import axiosClient from '../../../core/api/axiosClient'
import RegisterPage from './RegisterPage'

vi.mock('../../../core/api/axiosClient', () => ({
    default: {
        post: vi.fn(),
    },
}))

function renderRegisterPage() {
    return render(
        <MemoryRouter initialEntries={['/register']}>
            <Routes>
                <Route path="/register" element={<RegisterPage />} />
                <Route path="/login" element={<p>Login page</p>} />
            </Routes>
        </MemoryRouter>,
    )
}

async function completeRegistrationForm(
    user,
    {
        fullName = 'Test User',
        email = 'user@example.com',
        password = 'Password123!',
        confirmPassword = 'Password123!',
    } = {},
) {
    await user.type(screen.getByLabelText('Full name'), fullName)
    await user.type(screen.getByLabelText('Email address'), email)
    await user.type(screen.getByLabelText('Password'), password)
    await user.type(
        screen.getByLabelText('Confirm password'),
        confirmPassword,
    )
}

describe('RegisterPage', () => {
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
        vi.useRealTimers()
    })

    // Verifies that the registration page displays all required controls.
    it('renders the complete registration form', () => {
        renderRegisterPage()

        expect(
            screen.getByRole('heading', { name: 'Create your account' }),
        ).toBeInTheDocument()
        expect(screen.getByLabelText('Full name')).toBeInTheDocument()
        expect(screen.getByLabelText('Email address')).toBeInTheDocument()
        expect(screen.getByLabelText('Password')).toBeInTheDocument()
        expect(
            screen.getByLabelText('Confirm password'),
        ).toBeInTheDocument()
        expect(
            screen.getByRole('button', { name: 'Create account' }),
        ).toBeInTheDocument()
        expect(
            screen.getByRole('link', { name: 'Sign in' }),
        ).toHaveAttribute('href', '/login')
    })

    // Verifies that users can enter information in every registration field.
    it('updates every field when the user types', () => {
        renderRegisterPage()

        const fullNameInput = screen.getByLabelText('Full name')
        const emailInput = screen.getByLabelText('Email address')
        const passwordInput = screen.getByLabelText('Password')
        const confirmPasswordInput = screen.getByLabelText('Confirm password')

        fireEvent.change(fullNameInput, {
            target: { value: 'Test User' },
        })
        fireEvent.change(emailInput, {
            target: { value: 'user@example.com' },
        })
        fireEvent.change(passwordInput, {
            target: { value: 'Password123!' },
        })
        fireEvent.change(confirmPasswordInput, {
            target: { value: 'Password123!' },
        })

        expect(fullNameInput).toHaveValue('Test User')
        expect(emailInput).toHaveValue('user@example.com')
        expect(passwordInput).toHaveValue('Password123!')
        expect(confirmPasswordInput).toHaveValue('Password123!')
    })

    // Verifies that required empty fields prevent an API request.
    it('does not submit when required fields are empty', async () => {
        const user = userEvent.setup()
        renderRegisterPage()

        await user.click(
            screen.getByRole('button', { name: 'Create account' }),
        )

        expect(axiosClient.post).not.toHaveBeenCalled()
    })

    // Verifies that different passwords display a validation error.
    it('rejects registration when the passwords do not match', async () => {
        const user = userEvent.setup()
        renderRegisterPage()

        await completeRegistrationForm(user, {
            confirmPassword: 'Different123!',
        })

        await user.click(
            screen.getByRole('button', { name: 'Create account' }),
        )

        expect(await screen.findByRole('alert')).toHaveTextContent(
            'Passwords do not match.',
        )
        expect(axiosClient.post).not.toHaveBeenCalled()
    })

    // Verifies that passwords shorter than six characters are rejected.
    it('rejects a password containing fewer than six characters', async () => {
        const user = userEvent.setup()
        renderRegisterPage()

        await completeRegistrationForm(user, {
            password: '12345',
            confirmPassword: '12345',
        })

        const form = screen
            .getByRole('button', { name: 'Create account' })
            .closest('form')

        fireEvent.submit(form)

        expect(await screen.findByRole('alert')).toHaveTextContent(
            'Password must be at least 6 characters.',
        )
        expect(axiosClient.post).not.toHaveBeenCalled()
    })

    // Verifies the submitted API data, success message, and login redirection.
    it('registers successfully and redirects to login', async () => {
        vi.useFakeTimers()

        let resolveRegistration

        axiosClient.post.mockReturnValue(
            new Promise((resolve) => {
                resolveRegistration = resolve
            }),
        )

        renderRegisterPage()

        fireEvent.change(screen.getByLabelText('Full name'), {
            target: { value: '  Test User  ' },
        })
        fireEvent.change(screen.getByLabelText('Email address'), {
            target: { value: '  user@example.com  ' },
        })
        fireEvent.change(screen.getByLabelText('Password'), {
            target: { value: 'Password123!' },
        })
        fireEvent.change(screen.getByLabelText('Confirm password'), {
            target: { value: 'Password123!' },
        })

        const form = screen
            .getByRole('button', { name: 'Create account' })
            .closest('form')

        fireEvent.submit(form)

        expect(axiosClient.post).toHaveBeenCalledWith('/auth/register', {
            fullName: 'Test User',
            email: 'user@example.com',
            password: 'Password123!',
        })

        await act(async () => {
            resolveRegistration({ data: {} })
        })

        expect(screen.getByRole('status')).toHaveTextContent(
            'Account created successfully. Redirecting to login...',
        )

        await act(async () => {
            await vi.advanceTimersByTimeAsync(1200)
        })

        expect(screen.getByText('Login page')).toBeInTheDocument()
    })

    // Verifies that the button is disabled while registration is processing.
    it('shows a loading state while the request is running', async () => {
        axiosClient.post.mockReturnValue(new Promise(() => { }))

        const user = userEvent.setup()
        renderRegisterPage()

        await completeRegistrationForm(user)

        await user.click(
            screen.getByRole('button', { name: 'Create account' }),
        )

        expect(
            screen.getByRole('button', { name: 'Creating account...' }),
        ).toBeDisabled()
    })

    // Updated: uses fast events with a targeted timeout for slower test environments.
    it('displays a friendly message when the email already exists', async () => {
        const testEmail = ['user', 'example.com'].join('@')

        axiosClient.post.mockRejectedValue({
            response: {
                data: {
                    message: 'User already exists.',
                },
            },
        })

        renderRegisterPage()

        fireEvent.change(screen.getByLabelText('Full name'), {
            target: { value: 'Test User' },
        })
        fireEvent.change(screen.getByLabelText('Email address'), {
            target: { value: testEmail },
        })
        fireEvent.change(screen.getByLabelText('Password'), {
            target: { value: 'Password123!' },
        })
        fireEvent.change(screen.getByLabelText('Confirm password'), {
            target: { value: 'Password123!' },
        })

        const form = screen
            .getByRole('button', { name: 'Create account' })
            .closest('form')

        fireEvent.submit(form)

        expect(await screen.findByRole('alert')).toHaveTextContent(
            'This email is already registered. Please sign in instead.',
        )
    }, 10000)

    // Verifies that an error title returned by the API is displayed.
    it('displays the API error title when registration fails', async () => {
        axiosClient.post.mockRejectedValue({
            response: {
                data: {
                    title: 'Registration service unavailable.',
                },
            },
        })

        renderRegisterPage()

        fireEvent.change(screen.getByLabelText('Full name'), {
            target: { value: 'Test User' },
        })
        fireEvent.change(screen.getByLabelText('Email address'), {
            target: { value: 'user@example.com' },
        })
        fireEvent.change(screen.getByLabelText('Password'), {
            target: { value: 'Password123!' },
        })
        fireEvent.change(screen.getByLabelText('Confirm password'), {
            target: { value: 'Password123!' },
        })

        const form = screen
            .getByRole('button', { name: 'Create account' })
            .closest('form')

        fireEvent.submit(form)

        expect(await screen.findByRole('alert')).toHaveTextContent(
            'Registration service unavailable.',
        )
        expect(screen.queryByRole('status')).not.toBeInTheDocument()
    })

    // Verifies that the sign-in link opens the login page.
    it('navigates to login when the sign-in link is selected', async () => {
        const user = userEvent.setup()
        renderRegisterPage()

        await user.click(screen.getByRole('link', { name: 'Sign in' }))

        expect(screen.getByText('Login page')).toBeInTheDocument()
    })


})