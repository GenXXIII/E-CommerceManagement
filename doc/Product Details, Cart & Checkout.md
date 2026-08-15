# FullStack E-Commerce UI/UX Specification

## Part 1 — Product Foundation, Design System, Frontend Architecture, and Fixed Login

**Document:** `FullStack_ECommerce_UIUX.md`  
**Status:** Phase 1 UI/UX and frontend implementation specification  
**Primary source:** Existing full-stack e-commerce backend specification  
**Authentication mode for this phase:** Fixed demo login; replaceable later with Keycloak  
**Data policy:** Real business data from the ASP.NET Core API and SQL Server; no runtime mock business records  
**Frontend:** React + TypeScript + Vite  
**UI system:** Tailwind CSS + shadcn/ui + Radix UI primitives  
**Server state:** TanStack Query  
**Forms:** React Hook Form + Zod  
**Icons:** Lucide React  
**Charts:** Recharts  

---

## 1. Purpose and Scope

This document defines the frontend product experience and implementation rules for the e-commerce application described by the existing backend specification.

The application is an original marketplace-style storefront inspired by familiar large-scale commerce patterns.

The visual identity, copy, interaction patterns, component composition, and implementation details must remain original.

The application must not copy eBay logos, trademarks, proprietary layouts, copyrighted imagery, or proprietary content.

The experience should retain the useful characteristics of a high-volume marketplace:

- Search-first navigation.
- Broad category discovery.
- Dense but readable product cards.
- Strong filtering and sorting.
- Clear pricing and availability.
- Fast product comparison.
- Persistent cart access.
- Wishlist access.
- Straightforward checkout.
- Order visibility.
- Customer self-service.
- Separate administration workflows.

The first implementation phase intentionally uses a fixed demo login instead of Keycloak.

The fixed login exists only to unlock authenticated UI areas while the marketplace application is being built.

The fixed login must not change the underlying data architecture.

The application must still use real HTTP requests and persisted backend data for products, categories, carts, wishlists, addresses, orders, payments, refunds, reviews, inventory, and reports.

The fixed login layer must be isolated behind an authentication abstraction so it can later be replaced by Keycloak with minimal UI changes.

---

## 2. Phase 1 Authentication Decision

### 2.1 Fixed Login Is Temporary

Phase 1 uses a small frontend authentication adapter with fixed demo credentials.

The implementation is intended for local development and UX integration.

It is not a production security mechanism.

It must never be described as production authentication.

It must be easy to remove without rewriting feature modules.

The backend may also expose a development-only login endpoint if the implementation requires a backend session.

The preferred Phase 1 approach is to keep the authentication abstraction on the frontend and use the existing API directly for business data.

If the backend requires an identity header or development principal, that mechanism must be isolated in the API client.

No feature component should inspect hardcoded usernames or passwords.

No feature component should contain authentication rules.

### 2.2 Demo Accounts

The Phase 1 frontend may provide two demo identities.

| Username | Password | Display Name | Intended UI Area |
|---|---|---|---|
| `customer` | `customer123` | Demo Customer | Customer-facing marketplace and account UI |
| `admin` | `admin123` | Demo Administrator | Customer UI plus admin UI |

The exact credentials should live in one development-only configuration file.

They must not be scattered throughout components.

They must not be embedded in API functions.

They must not be displayed on public production builds.

Production builds must fail closed if the fixed-login feature is enabled accidentally.

### 2.3 Fixed Login Data Shape

Use a small internal type.

```ts
export type DemoRole = "customer" | "admin";

export interface AuthSession {
  isAuthenticated: boolean;
  username: string | null;
  displayName: string | null;
  role: DemoRole | null;
}
```

The session is UI state only during Phase 1.

The session must not be treated as authoritative authorization by the backend.

The UI may use the role to decide which navigation items to show in development.

The backend must still validate all real business mutations when production authentication is introduced.

### 2.4 Fixed Login Storage

Prefer in-memory session state during normal application execution.

A short-lived session marker may be stored in sessionStorage for local developer convenience.

Do not use localStorage as the source of truth for business data.

Do not store product records in localStorage.

Do not store cart totals in localStorage.

Do not store order status in localStorage.

Do not store payment status in localStorage.

Do not store inventory levels in localStorage.

The future Keycloak implementation should replace only the authentication adapter and provider.

Feature hooks, pages, and UI components should remain unchanged.

---

## 3. Source-of-Truth Rules

The backend specification requires real API and database data.

This document preserves that rule.

The frontend is never the authoritative source for business data.

The following must come from the API:

- Product IDs.
- Product names.
- Product descriptions.
- Product prices.
- Product stock quantities.
- Product activation status.
- Product categories.
- Product images when implemented.
- Product review summaries.
- Cart contents.
- Cart quantities.
- Cart totals.
- Wishlist contents.
- Customer profile data.
- Address records.
- Order numbers.
- Order status.
- Order totals.
- Payment status.
- Refund status.
- Inventory transactions.
- Sales statistics.
- Report metrics.
- Analytics values.

The frontend may hold transient UI state.

Examples of valid UI-only state include:

- Modal open or closed state.
- Selected tab.
- Expanded accordion.
- Search text before submission.
- Currently selected filters.
- Current pagination state before the request executes.
- Temporary form input.
- Hover and focus state.
- Theme preference.
- Table column visibility preference.

The frontend must not invent business state.

The frontend must not generate business IDs.

The frontend must not generate order totals as authoritative values.

The frontend must not generate payment outcomes.

The frontend must not decrement inventory directly.

The frontend must not create fake KPIs.

---

## 4. Product Experience Goals

The product should feel like a modern, trustworthy marketplace.

The main experience should be discoverable within seconds.

The primary user journey is:

```text
Open storefront
    -> Discover categories or search
    -> Search or browse products
    -> Filter and sort
    -> Open product details
    -> Add to wishlist or cart
    -> Review cart
    -> Checkout
    -> Complete development payment
    -> View order
```

The customer should always understand:

- Where they are.
- What they are looking at.
- What they can do next.
- Whether an action succeeded.
- Whether an action failed.
- Whether data is loading.
- Whether there are no results.
- Whether an item is unavailable.

The admin should always understand:

- Which module is open.
- Which records are currently displayed.
- Which filters are active.
- Which records are selected.
- What action is available.
- What action has already completed.
- Whether metrics are real and current.

---

## 5. UX Principles

### 5.1 Search First

Search is a primary navigation mechanism.

The global search field is visible on desktop.

The global search field is easily reachable on mobile.

Search suggestions should reduce typing without replacing the real product search API.

Search autocomplete must use API results or derived values from already loaded real data.

Search suggestions must never use a hardcoded fake product list.

### 5.2 Progressive Disclosure

Do not show every possible control at once.

Show the most important action first.

Secondary actions may live in menus or overflow controls.

Desktop may expose filters in a side rail.

Tablet may use a collapsible filter panel.

Mobile should use a filter drawer or sheet.

### 5.3 Clear Hierarchy

Use typography, spacing, and grouping to establish visual hierarchy.

The most important information on a product card is:

1. Product image.
2. Product name.
3. Price.
4. Availability.
5. Primary action.

The most important information on an order card is:

1. Order number.
2. Order date.
3. Current status.
4. Total.
5. Primary next action.

### 5.4 Honest Feedback

Every async action must expose a visible result.

Examples:

- Loading.
- Success.
- Validation failure.
- Business conflict.
- Unauthorized.
- Not found.
- Rate limited.
- Server error.

Avoid silent failures.

Avoid success messages for requests that have not completed.

Avoid optimistic updates when the server result materially determines correctness unless rollback is reliable.

### 5.5 Empty Data Is a Valid State

An empty database is not an error.

An empty search result is not a system failure.

An empty wishlist is not an error.

An empty order history is not an error.

The UI should explain what is empty and provide an appropriate next action.

The UI must never insert sample records to make an empty state look populated.

---

## 6. Original Brand Direction

The application should use an original commerce brand.

Recommended working name for the UI specification:

**MarketNest**

The brand name is a placeholder for the implementation team and may be replaced later.

The interface should communicate:

- Reliability.
- Variety.
- Convenience.
- Speed.
- Transparency.

The visual identity should use a clean neutral base with one strong primary accent and one supporting accent.

Avoid copying the familiar multicolor letter treatment associated with major marketplace brands.

Avoid using their trademark color combinations as a brand signature.

Avoid recognizable competitor logos.

Use original icon combinations and illustrations.

### 6.1 Brand Voice

Brand copy should be:

- Clear.
- Concise.
- Helpful.
- Neutral.
- Action-oriented.
- Non-pushy.

Avoid exaggerated claims unless supported by real backend data.

Do not use statements such as “millions sold” unless a real API statistic supports the statement.

Do not display “best seller” labels unless a backend rule provides that classification.

Do not display fake discount percentages.

Do not display fake review counts.

---

## 7. Visual Design System

### 7.1 Design Language

Use a high-information marketplace interface with generous whitespace around major sections.

The design should feel dense in data areas and spacious in navigation areas.

The storefront should prioritize scanning.

The admin interface should prioritize operational clarity.

Use neutral surfaces for most content.

Reserve accent colors for actions, active states, links, and meaningful status indicators.

### 7.2 Base Visual Tokens

```text
Base background: near-white neutral
Surface: white
Surface muted: light neutral
Border: subtle neutral
Text primary: deep neutral
Text secondary: muted neutral
Text disabled: low-contrast neutral
Primary action: strong accent
Primary action hover: darker accent
Secondary action: neutral outline
Success: semantic green family
Warning: semantic amber family
Danger: semantic red family
Info: semantic blue family
```

Exact token values should be defined in Tailwind theme variables.

Do not scatter raw color values throughout components.

### 7.3 Color Token Naming

Use semantic names.

```text
--background
--foreground
--card
--card-foreground
--popover
--popover-foreground
--primary
--primary-foreground
--secondary
--secondary-foreground
--muted
--muted-foreground
--accent
--accent-foreground
--destructive
--destructive-foreground
--border
--input
--ring
```

Add application-specific semantic tokens for:

```text
--success
--success-foreground
--warning
--warning-foreground
--info
--info-foreground
--price
--stock-in
--stock-low
--stock-out
```

Use semantic tokens instead of hardcoding utility combinations in every component.

### 7.4 Contrast Rules

Text must maintain accessible contrast.

Interactive controls must have visible focus states.

Disabled controls may have reduced contrast but should remain understandable.

Do not communicate meaning using color alone.

Status badges should combine color with text.

Stock indicators should combine color with labels.

Validation states should include text or icons with accessible labels.

---

## 8. Typography System

Use a modern sans-serif system font stack by default.

Recommended stack:

```css
font-family:
  Inter,
  ui-sans-serif,
  system-ui,
  -apple-system,
  BlinkMacSystemFont,
  "Segoe UI",
  sans-serif;
```

If a branded font is added later, use it only for major headings and brand surfaces.

Do not use more than two font families.

### 8.1 Type Scale

```text
Display: 48px / 56px / 700
Heading XL: 36px / 44px / 700
Heading LG: 30px / 38px / 700
Heading MD: 24px / 32px / 700
Heading SM: 20px / 28px / 600
Body LG: 18px / 28px / 400
Body: 16px / 24px / 400
Body SM: 14px / 20px / 400
Caption: 12px / 16px / 500
```

Use responsive type scaling where appropriate.

Do not use very small body text for critical product or checkout information.

### 8.2 Product Price Typography

Primary product prices should be visually prominent.

Use a strong weight.

Keep currency and amount together.

Use consistent decimal formatting based on the API currency contract.

Do not assume a currency in the UI if the backend contract is not finalized.

Centralize currency formatting in a shared component.

### 8.3 Text Truncation

Product names may be truncated on compact cards.

Never truncate the primary product title on the detail page.

Use tooltips only when truncated text is difficult to understand.

Prefer line clamping to hidden overflow when it improves scanning.

---

## 9. Spacing System

Use a consistent spacing scale.

Preferred base unit: 4px.

Recommended spacing tokens:

```text
0: 0px
1: 4px
2: 8px
3: 12px
4: 16px
5: 20px
6: 24px
8: 32px
10: 40px
12: 48px
16: 64px
20: 80px
24: 96px
```

Prefer Tailwind spacing utilities.

Avoid arbitrary values unless the design requires a unique measurement.

Use consistent vertical rhythm between section headings and content.

### 9.1 Content Spacing

Page shell padding:

- Desktop: 24px to 40px.
- Tablet: 20px to 24px.
- Mobile: 16px.

Section spacing:

- Desktop: 48px to 72px.
- Tablet: 40px to 56px.
- Mobile: 32px to 40px.

Card internal padding:

- Standard: 16px.
- Compact: 12px.
- Large feature card: 20px to 24px.

---

## 10. Layout Grid

Use a centered max-width content container.

Recommended maximum width:

```text
max-width: 1440px
```

The desktop storefront should support:

- Full-width header.
- Centered content shell.
- Optional wide promotional areas.
- Product grid from 4 to 6 columns depending on viewport.

Suggested layout widths:

```text
Sidebar: 240px to 280px
Content gap: 24px
Main content: remaining width
```

On product listing pages:

```text
Desktop:
[Filter rail][Product grid]

Tablet:
[Filter button][Product grid]

Mobile:
[Filter button][Sort button]
[Product grid]
```

Do not force desktop sidebars onto narrow screens.

---

## 11. Responsive Breakpoints

Use Tailwind breakpoints unless the implementation team has a strong reason to customize them.

Recommended behavior:

```text
sm: 640px
md: 768px
lg: 1024px
xl: 1280px
2xl: 1536px
```

### 11.1 Mobile

Primary concerns:

- Thumb-friendly controls.
- Minimal header height.
- Full-width search.
- Drawer-based navigation.
- One-column product cards.
- Bottom-accessible primary actions.

### 11.2 Tablet

Primary concerns:

- Two-column or three-column product grids.
- Collapsible filters.
- Condensed utility navigation.
- Search remains prominent.

### 11.3 Desktop

Primary concerns:

- Full navigation.
- Persistent category access.
- Multi-column product grids.
- Side filter rail.
- Dense admin tables.

---

## 12. Tailwind CSS Standards

Tailwind CSS is the default styling mechanism.

Prefer utility classes composed through reusable components.

Do not create large global CSS files for component-specific layout.

Use CSS variables for theme tokens.

Use `cn()` for conditional class composition.

Use `cva` where variants become complex.

### 12.1 Utility Composition Rules

Prefer semantic component variants over repeated utility strings.

Example:

```ts
const buttonVariants = cva(
  "inline-flex items-center justify-center rounded-md font-medium transition-colors focus-visible:outline-none focus-visible:ring-2",
  {
    variants: {
      variant: {
        primary: "bg-primary text-primary-foreground hover:bg-primary/90",
        secondary: "border bg-background hover:bg-muted",
        destructive: "bg-destructive text-destructive-foreground hover:bg-destructive/90",
      },
      size: {
        sm: "h-9 px-3 text-sm",
        md: "h-10 px-4",
        lg: "h-11 px-6",
      },
    },
    defaultVariants: {
      variant: "primary",
      size: "md",
    },
  },
);
```

### 12.2 Avoid Arbitrary CSS Everywhere

Avoid patterns such as:

```text
text-[#123456]
p-[17px]
mt-[13px]
```

Use design tokens and standard spacing whenever possible.

Arbitrary values are acceptable only for documented exceptions.

### 12.3 Layout Rules

Prefer CSS Grid for page-level structure.

Prefer Flexbox for linear component layout.

Avoid deeply nested wrappers that provide no layout purpose.

Use `gap` instead of margin chains for repeated layouts.

Keep content widths predictable.

---

## 13. Recommended Tailwind Theme Structure

The Tailwind theme should define semantic color variables and spacing behavior.

Suggested conceptual configuration:

```ts
export default {
  theme: {
    extend: {
      colors: {
        border: "hsl(var(--border))",
        background: "hsl(var(--background))",
        foreground: "hsl(var(--foreground))",
        primary: {
          DEFAULT: "hsl(var(--primary))",
          foreground: "hsl(var(--primary-foreground))",
        },
        secondary: {
          DEFAULT: "hsl(var(--secondary))",
          foreground: "hsl(var(--secondary-foreground))",
        },
        destructive: {
          DEFAULT: "hsl(var(--destructive))",
          foreground: "hsl(var(--destructive-foreground))",
        },
        muted: {
          DEFAULT: "hsl(var(--muted))",
          foreground: "hsl(var(--muted-foreground))",
        },
      },
      borderRadius: {
        lg: "var(--radius)",
        md: "calc(var(--radius) - 2px)",
        sm: "calc(var(--radius) - 4px)",
      },
    },
  },
};
```

The exact configuration can follow the installed Tailwind version.

The implementation must not blindly copy this example if the installed Tailwind version uses a different configuration mechanism.

---

## 14. Border Radius System

Use rounded corners consistently.

Recommended:

```text
sm: 6px
md: 8px
lg: 12px
xl: 16px
2xl: 20px
pill: 9999px
```

Use smaller radius for:

- Inputs.
- Buttons.
- Compact controls.

Use medium radius for:

- Cards.
- Dropdowns.
- Dialogs.

Use larger radius for:

- Hero sections.
- Large promotional surfaces.
- Mobile bottom sheets.

Avoid excessive pill styling for every control.

Pills should be reserved for chips, tags, and compact statuses.

---

## 15. Elevation and Shadows

Use subtle shadows.

Preferred hierarchy:

```text
Level 0: no shadow
Level 1: subtle card elevation
Level 2: dropdown and popover elevation
Level 3: dialog and drawer elevation
Level 4: floating navigation or high-priority overlays
```

Avoid heavy shadows on every card.

Use borders to separate dense data surfaces.

Use elevation to establish layering, not decoration.

---

## 16. shadcn/ui Component Standards

Use shadcn/ui components as the base for accessible primitives.

Recommended components:

- Button.
- Input.
- Label.
- Textarea.
- Select.
- Checkbox.
- RadioGroup.
- Switch.
- Dialog.
- Sheet.
- Drawer.
- DropdownMenu.
- Popover.
- Command.
- Tooltip.
- Tabs.
- Accordion.
- Card.
- Badge.
- Alert.
- Skeleton.
- Separator.
- Breadcrumb.
- Pagination.
- Table.
- Calendar.
- Form.

Do not duplicate primitives in feature folders.

The `components/ui` directory should contain shared presentation primitives.

Feature-specific wrappers belong in feature folders.

Example:

```text
components/ui/Button.tsx
features/products/components/ProductCard.tsx
features/products/components/ProductFilters.tsx
```

Do not create a second Button implementation inside `features/products`.

---

## 17. Component Variant Rules

Component variants must communicate purpose.

Avoid arbitrary combinations of classes in pages.

Use named variants.

Examples:

```text
Button: primary | secondary | outline | ghost | destructive
Badge: default | success | warning | danger | info
Card: default | elevated | interactive | compact
StatusBadge: pending | confirmed | shipped | delivered | cancelled
```

The same semantic status must have the same visual treatment across the product.

A `Delivered` status should not appear green in one page and blue in another.

---

## 18. Lucide Icon Standards

Use Lucide React for interface icons.

Do not use emoji as functional interface icons.

Icons must be visually consistent in size.

Recommended icon sizes:

```text
12px: very compact metadata
14px: dense table controls
16px: standard controls
18px: prominent buttons
20px: header actions
24px: mobile navigation
```

Icon-only buttons must have accessible names.

Use `aria-label` when visible text is absent.

Avoid using icons as the only indicator for destructive actions.

---

## 19. Motion System

Animations must reinforce hierarchy and feedback.

Recommended durations:

```text
Micro interaction: 100ms to 150ms
Standard transition: 150ms to 250ms
Panel transition: 200ms to 300ms
Page-level motion: 250ms to 400ms
```

Use easing that feels natural.

Avoid continuous decorative motion.

Avoid bouncing buttons.

Avoid animations that delay checkout or critical actions.

### 19.1 Recommended Motion

Use subtle animation for:

- Search dropdown appearance.
- Dialog entry.
- Sheet entry.
- Product card hover elevation.
- Wishlist toggle feedback.
- Cart item removal.
- Toast appearance.
- Skeleton shimmer.

### 19.2 Reduced Motion

Respect `prefers-reduced-motion`.

Disable nonessential motion when the user requests reduced motion.

Do not rely on motion to communicate status.

---

## 20. Accessibility Requirements

Target WCAG 2.2 AA for the interface.

### 20.1 Keyboard Navigation

Every interactive feature must be keyboard accessible.

Keyboard focus must be visible.

Dialogs must trap focus appropriately.

Dropdowns must support keyboard navigation.

Search autocomplete must support arrow-key selection.

Escape should close dismissible overlays.

### 20.2 Screen Reader Support

Use semantic HTML.

Prefer `button` for actions.

Prefer `a` for navigation.

Do not use clickable `div` elements for primary interactions.

Provide labels for inputs.

Use `aria-live` sparingly for async feedback.

Use status regions for save success and failure messages where appropriate.

### 20.3 Forms

Every form field must have a visible or programmatically associated label.

Validation errors must identify the field.

Error text must not depend only on color.

Use `aria-describedby` for detailed field guidance and errors.

### 20.4 Tables

Admin tables must:

- Use semantic table markup.
- Provide column headers.
- Support keyboard navigation when interactive.
- Avoid extremely dense touch targets on mobile.

Mobile tables may become card lists when a tabular layout is no longer practical.

---

## 21. React + TypeScript Architecture

Use feature-based architecture.

Avoid organizing the application only by technical type.

Business features should own their API functions, query keys, hooks, components, schemas, and types.

Recommended structure:

```text
src/
├── app/
│   ├── providers/
│   ├── router/
│   └── App.tsx
├── components/
│   ├── ui/
│   ├── common/
│   └── feedback/
├── core/
│   ├── api/
│   ├── auth/
│   ├── config/
│   ├── constants/
│   ├── types/
│   ├── utils/
│   └── validation/
├── features/
│   ├── categories/
│   ├── products/
│   ├── search/
│   ├── cart/
│   ├── wishlist/
│   ├── checkout/
│   ├── orders/
│   ├── payments/
│   ├── refunds/
│   ├── reviews/
│   ├── profile/
│   ├── addresses/
│   ├── inventory/
│   ├── users/
│   └── reports/
├── layouts/
│   ├── StorefrontLayout.tsx
│   ├── AccountLayout.tsx
│   └── AdminLayout.tsx
├── pages/
│   ├── public/
│   ├── account/
│   └── admin/
├── lib/
│   └── utils.ts
└── main.tsx
```

The final project should also follow the backend specification's monorepo structure.

The frontend remains separate from the ASP.NET Core solution.

---

## 22. Feature Folder Contract

Every mature feature should follow a predictable structure.

Example:

```text
features/products/
├── api/
│   ├── productApi.ts
│   └── productKeys.ts
├── components/
│   ├── ProductCard.tsx
│   ├── ProductGrid.tsx
│   ├── ProductFilters.tsx
│   └── ProductSort.tsx
├── hooks/
│   ├── useProductSearch.ts
│   └── useProduct.ts
├── schemas/
│   └── productSchemas.ts
├── types/
│   └── product.types.ts
└── utils/
    └── productFormatters.ts
```

Rules:

- API functions belong in `api`.
- TanStack Query hooks belong in `hooks`.
- Zod schemas belong in `schemas`.
- DTO and UI model types belong in `types`.
- Reusable feature components belong in `components`.
- Feature-specific formatting logic belongs in `utils`.

Do not create a `mocks` directory.

Do not create runtime fixtures.

---

## 23. Core API Layer

React components must never call `fetch` or Axios directly.

The request flow must be:

```text
Page
  -> Feature Hook
  -> Feature API Function
  -> Core API Client
  -> ASP.NET Core API
```

Example:

```ts
export const productKeys = {
  all: ["products"] as const,
  search: (params: ProductSearchParams) =>
    ["products", "search", params] as const,
  detail: (id: string) =>
    ["products", "detail", id] as const,
};
```

The API client should own:

- Base URL.
- Request headers.
- Development authentication context.
- JSON serialization.
- Response parsing.
- Error normalization.
- Request cancellation.
- Correlation ID handling if supported.

The API client should not contain business-specific transformation logic.

Feature API functions should map backend endpoints to typed frontend contracts.

---

## 24. Typed API Contract Rules

All API functions should have explicit return types.

Avoid `any`.

Use generated OpenAPI types where practical.

If code generation is not enabled, define local DTO types that accurately match the backend.

Do not silently change backend enum names in API types.

If a UI label differs from a backend enum, transform it in a presentation helper.

Example:

```ts
export type OrderStatus =
  | "Pending"
  | "Confirmed"
  | "Packed"
  | "Shipped"
  | "Delivered"
  | "Cancelled"
  | "PendingPayment"
  | "PaymentFailed";
```

UI labels should be mapped centrally.

Never duplicate status mappings across pages.

---

## 25. TanStack Query Strategy

TanStack Query is the source of client-side server-state caching.

It is not the source of truth.

The backend remains authoritative.

Use query keys that encode all relevant server parameters.

Example:

```ts
["products", "search", {
  keyword,
  categoryId,
  page,
  pageSize,
}]
```

After mutations:

- Invalidate affected queries.
- Update cached data only when the server response is authoritative.
- Avoid manually reconstructing complex business totals.

Examples:

Adding an item to cart should invalidate the cart query.

Changing cart quantity should invalidate or update the cart query with the returned response.

Creating a product should invalidate the product list and any relevant category queries.

Updating inventory should invalidate inventory history and any affected product availability query.

### 25.1 Query Defaults

Use conservative defaults.

Do not poll every endpoint continuously.

Use refetch-on-focus selectively.

Use stale times based on expected volatility.

Suggested direction:

```text
Categories: relatively long stale time
Product details: moderate stale time
Product search: moderate stale time
Cart: short stale time
Orders: short to moderate stale time
Inventory: short stale time
Reports: controlled refresh
```

Exact values should be tuned after observing API behavior.

---

## 26. API Error Normalization

Normalize backend failures into one frontend error shape.

Example:

```ts
export interface ApiError {
  status: number;
  title?: string;
  detail?: string;
  traceId?: string;
  errors?: Record<string, string[]>;
}
```

Map status codes consistently.

```text
400 -> validation or malformed request
401 -> fixed-login session unavailable in Phase 1 or future authentication failure
403 -> forbidden state when backend authorization is active
404 -> not found state
409 -> business conflict
429 -> rate limit state
500 -> server error
503 -> service unavailable
```

During Phase 1 fixed login, `401` should be handled as an authentication/session problem.

The UI must not claim “you do not have permission” unless the backend returned a relevant authorization response.

---

## 27. Loading State System

Every API-driven screen must have a loading state.

Prefer skeletons for predictable content areas.

Use spinners for compact actions.

Avoid replacing the entire page with a spinner after every small mutation.

### 27.1 Product Loading

Product cards should use image, title, price, and metadata skeletons.

### 27.2 Table Loading

Tables should render a skeleton row structure matching the expected columns.

### 27.3 Detail Loading

Use image gallery skeletons and content placeholders.

### 27.4 Mutation Loading

Buttons performing mutations should show loading text or a spinner.

Disable duplicate submission while a mutation is pending.

---

## 28. Empty State System

All empty states should include:

- Clear heading.
- One-sentence explanation.
- Appropriate visual icon or illustration.
- Next action when available.

Examples:

```text
No products found
Try a different keyword or remove a filter.
[Clear filters]
```

```text
Your wishlist is empty
Save products here to compare or revisit them later.
[Browse products]
```

```text
No orders yet
Completed purchases will appear here.
[Start shopping]
```

Never include fake products in empty states.

---

## 29. Global Application Layout

The storefront layout should contain:

```text
StorefrontLayout
├── UtilityBar
├── MainHeader
│   ├── BrandLogo
│   ├── CategoryMenu
│   ├── SearchBar
│   ├── WishlistAction
│   ├── AccountAction
│   └── CartAction
├── CategoryNav
├── MainContent
└── Footer
```

The admin layout should be separate.

The account layout should be separate.

Shared primitives may be reused.

Page-specific navigation should not leak into the global storefront header.

---

## 30. Desktop Header Specification

Desktop header has three conceptual layers.

### 30.1 Utility Row

Contains:

- Welcome text or sign-in state.
- Help link.
- Customer account shortcut.
- Wishlist shortcut.
- Admin shortcut when fixed demo role is admin.

Keep the row visually quiet.

### 30.2 Main Navigation Row

Contains:

- Brand.
- Shop by category control.
- Large search input.
- Category selector.
- Search button.
- Wishlist icon.
- Cart icon.

The search bar should occupy the largest horizontal area.

The cart count must come from the real cart query for authenticated demo sessions.

If the cart query is unavailable, show an explicit loading or unavailable state rather than an invented count.

### 30.3 Category Row

Display active categories loaded from the API.

Do not hardcode category names.

Category navigation should support horizontal scrolling when the number of categories exceeds available width.

---

## 31. Tablet Header Specification

Tablet layouts should simplify the header.

Use:

- Brand.
- Search.
- Cart.
- Menu trigger.

Place account and wishlist actions inside the menu when horizontal space is limited.

The category navigation may become horizontally scrollable.

Avoid wrapping the main header into multiple unpredictable lines.

---

## 32. Mobile Header Specification

Mobile header should contain:

```text
[Menu] [Brand] [Cart]
[Search field]
```

The search field should remain easy to access.

A category drawer should contain API-loaded categories.

The mobile menu should include:

- Home.
- Shop by category.
- Products.
- Wishlist.
- Cart.
- Account.
- Admin area for the demo admin user.

The fixed-login role should not be exposed as a security guarantee.

The menu is only a Phase 1 UI gate.

---

## 33. Search Bar Specification

The search bar is a global component.

Props should include:

```ts
interface SearchBarProps {
  value: string;
  onChange: (value: string) => void;
  onSubmit: () => void;
  placeholder?: string;
  disabled?: boolean;
}
```

Search submission should navigate to the product listing route with query parameters.

Example:

```text
/products?keyword=wireless+headphones&page=1&pageSize=24
```

The frontend should not execute local filtering across a large product dataset.

The backend search API remains authoritative.

---

## 34. Search Autocomplete UX

Autocomplete should appear after the user enters a meaningful query.

Use a small debounce.

Do not call the API on every keystroke without a debounce strategy.

Suggestions may include:

- Matching product names.
- Matching categories.
- Relevant recent search terms if stored as UI preference.

Product suggestions must come from real API data.

Do not fabricate suggestions.

### 34.1 Keyboard Behavior

Support:

- Arrow down.
- Arrow up.
- Enter.
- Escape.

The active suggestion must be visually distinct.

The component should use the appropriate combobox semantics.

### 34.2 Mobile Behavior

On mobile, autocomplete may open in a full-width sheet or inline panel.

Ensure the keyboard does not obscure the most relevant suggestions.

### 34.3 Search Result Navigation

Selecting a product suggestion should navigate directly to its real product detail route.

Selecting a category should navigate to the category listing route.

Submitting arbitrary text should navigate to the product search route.

---

## 35. Category Navigation

Categories are retrieved from the category API.

The navigation should support:

- Top categories.
- Category listing.
- Category detail.
- Category product count when supported by the backend.

Do not display product counts unless the API returns real counts.

Do not invent category hierarchy if the backend does not expose a hierarchy.

If hierarchy is later added, use a recursive navigation component.

---

## 36. Breadcrumb Standards

Use breadcrumbs on:

- Product details.
- Category pages.
- Account pages.
- Admin detail pages.

Example:

```text
Home / Electronics / Audio / Product Name
```

Breadcrumb items should use real category and product names.

Do not hardcode breadcrumb content.

The current page should not be an interactive link unless there is a clear navigational reason.

---

## 37. Footer Specification

The storefront footer should contain:

- About the marketplace.
- Customer service.
- Shopping help.
- Account links.
- Legal links.
- Contact information when provided by the application configuration.

Do not invent physical addresses, phone numbers, or support hours.

Footer links should either navigate to real pages or remain clearly marked as future content during development.

The footer should be responsive.

On mobile, use accordion sections.

On desktop, use multi-column groups.

---

## 38. Fixed Login Page

The fixed login page is a temporary Phase 1 experience.

Route:

```text
/login
```

The page should contain:

- Brand logo.
- Welcome heading.
- Username input.
- Password input.
- Sign-in button.
- Demo account hint in development builds only.
- Error message for invalid credentials.
- Link back to storefront.

The login form must use React Hook Form and Zod.

Validation:

- Username required.
- Password required.

Authentication flow:

```text
User submits login
    -> Validate form
    -> Check fixed development credentials
    -> Create AuthSession
    -> Store session in auth provider
    -> Navigate to intended route or home
```

### 38.1 Login Success

Show a brief success transition only if it does not delay navigation.

Navigate directly to the intended destination.

### 38.2 Login Failure

Use a generic error:

```text
Invalid username or password.
```

Do not reveal which credential is incorrect.

### 38.3 Logout

Logout should:

- Clear the session.
- Clear user-specific TanStack Query cache where appropriate.
- Navigate to the storefront.

Do not leave customer-specific cart or wishlist data visible after logout.

---

## 39. Fixed Login Architecture

Use an interface that can later be replaced.

```ts
export interface AuthAdapter {
  login(username: string, password: string): Promise<AuthSession>;
  logout(): Promise<void>;
  getSession(): AuthSession;
  isAuthenticated(): boolean;
}
```

Phase 1 implementation:

```ts
export class FixedDemoAuthAdapter implements AuthAdapter {
  // development-only implementation
}
```

Future implementation:

```ts
export class KeycloakAuthAdapter implements AuthAdapter {
  // later production authentication implementation
}
```

The rest of the application should depend on `AuthAdapter` or a React hook rather than the concrete class.

---

## 40. Auth Provider

The React provider should expose:

```ts
interface AuthContextValue {
  session: AuthSession;
  isLoading: boolean;
  login: (username: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
}
```

The provider should be mounted near the root application provider tree.

Recommended order:

```text
BrowserRouter
  -> QueryClientProvider
  -> AuthProvider
  -> AppRouter
```

The exact provider nesting may vary.

Avoid coupling the AuthProvider to feature APIs during Phase 1.

When Keycloak is added later, the provider can wrap the Keycloak adapter.

---

## 41. Route Strategy for Phase 1

Public routes:

```text
/
/products
/products/:productId
/categories/:categoryId
/login
```

Customer routes:

```text
/cart
/checkout
/payment/:paymentId
/payment/:paymentId/result
/account
/account/profile
/account/addresses
/account/wishlist
/account/orders
/account/orders/:orderId
/account/refunds
```

Admin routes:

```text
/admin
/admin/categories
/admin/products
/admin/inventory
/admin/orders
/admin/payments
/admin/refunds
/admin/reviews
/admin/customers
/admin/reports
```

Phase 1 route guards may use the fixed demo role.

The implementation should make route guards replaceable later.

---

## 42. Route Guard Abstraction

Use a generic guard.

```ts
interface RouteGuardProps {
  children: React.ReactNode;
  requireAuth?: boolean;
  requiredRole?: DemoRole;
}
```

Phase 1 behavior:

- `requireAuth=true` requires a fixed login session.
- `requiredRole="admin"` requires the demo admin session.

Future behavior:

- Authentication is determined by Keycloak.
- Roles are determined by validated token claims and backend policies.

The rest of the route configuration should not need structural changes.

---

## 43. Account Navigation

The account area should use a consistent navigation panel.

Sections:

- Overview.
- Profile.
- Addresses.
- Wishlist.
- Orders.
- Refunds.

Mobile behavior:

- Horizontal tab scrolling or dropdown navigation.

Desktop behavior:

- Left-side navigation panel.
- Main content panel.

The account layout must remain usable with an empty order history.

---

## 44. Admin Navigation Foundation

The admin layout should contain:

```text
AdminLayout
├── Sidebar
│   ├── Dashboard
│   ├── Categories
│   ├── Products
│   ├── Inventory
│   ├── Orders
│   ├── Payments
│   ├── Refunds
│   ├── Reviews
│   ├── Customers
│   └── Reports
├── Header
│   ├── Breadcrumbs
│   ├── Search or quick action area
│   └── Current user menu
└── MainContent
```

The admin sidebar should collapse on desktop.

On mobile, use a sheet or drawer.

The admin dashboard must not display hardcoded metrics.

A zero metric is valid.

An empty chart is valid.

A missing report endpoint should produce an explicit unavailable state rather than fake data.

---

## 45. Component Hierarchy Rules

Global components should remain low-level and reusable.

Feature components should express business concepts.

Pages should compose feature components.

Example:

```text
HomePage
├── HeroSearchSection
│   └── SearchBar
├── CategoryDiscovery
│   └── CategoryCard[]
├── RecentProductsSection
│   └── ProductGrid
│       └── ProductCard[]
└── EmptyOrFallbackSections
```

Product listing:

```text
ProductListingPage
├── PageHeader
├── SearchSummary
├── FilterToolbar
├── ProductResults
│   ├── LoadingSkeleton
│   ├── EmptyState
│   ├── ErrorState
│   └── ProductGrid
└── Pagination
```

Admin dashboard:

```text
AdminDashboardPage
├── PageHeader
├── KPIGrid
├── SalesOverviewChart
├── OrderStatusChart
├── LowStockTable
└── RecentOrdersTable
```

Every visual data point must be backed by real API data.

---

## 46. Common Feedback Components

Implement these reusable components early:

```text
LoadingSkeleton
ErrorState
EmptyState
NotFoundState
UnauthorizedState
InlineFieldError
ToastProvider
ConfirmDialog
StatusBadge
```

Each should have a consistent visual pattern.

### 46.1 ErrorState

Include:

- Clear title.
- Short explanation.
- Retry action when retry is meaningful.

### 46.2 EmptyState

Include:

- Clear empty heading.
- Helpful explanation.
- Relevant action.

### 46.3 ConfirmDialog

Use for:

- Remove cart item.
- Delete address.
- Remove wishlist item when a confirmation is appropriate.
- Admin destructive actions.

Do not use confirmation dialogs for every minor action.

---

## 47. Confirmation and Destructive Action Rules

Destructive actions must be visually clear.

Use a destructive variant for irreversible or potentially irreversible operations.

Examples:

- Delete category.
- Delete product.
- Delete address.
- Remove cart item.
- Reject refund.

The dialog should explain the consequence.

Do not use vague labels such as `OK`.

Prefer:

```text
Delete product
Cancel
```

Avoid destructive actions as the default primary button when a safer alternative exists.

---

## 48. Toast Notification Standards

Toasts are for lightweight feedback.

Use them for:

- Wishlist item added.
- Item removed.
- Cart item added.
- Profile saved.
- Address saved.
- Admin update completed.

Do not use toasts as the only validation mechanism.

Do not use toasts for critical payment errors that require user attention on the page.

Toasts should have accessible live-region behavior.

---

## 49. Data Freshness Indicators

For dashboards and reports, optionally show:

```text
Last updated: 2 minutes ago
```

Only when the API provides a meaningful timestamp or the frontend can safely identify the last successful fetch time.

Do not imply real-time data if the UI merely cached a previous response.

Refresh actions must trigger actual API refetches.

---

## 50. Performance Rules

Performance must support real API usage.

Use pagination for large collections.

Do not load all products to filter locally.

Use image lazy loading where appropriate.

Use responsive image sizes when supported.

Avoid rendering very large product grids without virtualization or pagination.

Avoid unnecessary query refetches.

Use route-level lazy loading for heavy admin pages.

Do not block first content render on optional analytics widgets.

### 50.1 Code Splitting

Recommended route groups:

```text
Public storefront bundle
Account bundle
Admin bundle
```

The admin bundle may be lazy-loaded.

The reports module may be lazy-loaded because charting libraries can be large.

---

## 51. Image and Media Rules

Use real image URLs returned by the backend.

Provide alt text based on product names and meaningful context.

Do not embed base64 product images in product DTOs.

When an image is unavailable, display a consistent neutral placeholder.

Do not generate fake product photography inside the frontend.

Do not use random image URLs from external services as runtime business content.

Use object-fit rules based on the image type.

Product thumbnails should generally use `object-contain` when the product itself should remain fully visible.

Lifestyle images may use `object-cover` when the backend supports them.

---

## 52. Product Card Foundation

The common product card must be reusable across:

- Home page.
- Search results.
- Category pages.
- Wishlist.
- Related products.
- Recently viewed sections.

The card may display:

- Product image.
- Product name.
- Price.
- Stock state.
- Rating summary when available.
- Wishlist control.
- Add-to-cart action.

Do not display:

- Random star ratings.
- Fake review counts.
- Fake sales counts.
- Fake discounts.
- Invented shipping promises.

### 52.1 Product Card Interaction

Clicking the product body navigates to the detail page.

Clicking the wishlist button must not navigate to the product detail page.

Clicking Add to Cart must not navigate unless the UX explicitly chooses a cart transition.

Use event propagation control carefully.

---

## 53. Product Listing Foundations

The product listing screen must support the backend's initial query contract.

Current baseline parameters:

```text
keyword
categoryId
page
pageSize
```

The UI should be architected to add later:

```text
minPrice
maxPrice
inStock
condition
brand
sortBy
sortDirection
```

The page must not expose controls that the backend does not support yet.

Controls should become visible only when corresponding API support exists.

---

## 54. Pagination Standards

Use server-side pagination.

Do not load every product and paginate in the browser.

Display:

- Current page.
- Total pages when returned.
- Previous.
- Next.
- Optional page-size selector.

On mobile, simplify the pagination control.

Example:

```text
[Previous]  Page 2 of 10  [Next]
```

All page changes should update the URL query parameters.

This enables shareable and bookmarkable search results.

---

## 55. URL State Rules

Use the URL for durable page state that should survive refresh.

Recommended URL state:

- Search keyword.
- Category.
- Page.
- Page size.
- Sort.
- Filters.

Use local component state for temporary UI state.

Do not put modal open state in the URL unless deep-linking is required.

Keep query parameter names consistent across pages.

---

## 56. Form Architecture

Use React Hook Form for complex forms.

Use Zod for client-side validation.

Client-side validation improves user experience.

It does not replace backend validation.

The server remains authoritative.

### 56.1 Form Submission Flow

```text
User input
    -> React Hook Form
    -> Zod validation
    -> API request
    -> Backend validation
    -> Success or normalized error
    -> UI feedback
```

Map backend validation errors to the correct fields when possible.

Show a generic form error when the backend returns a non-field error.

---

## 57. Mobile Touch Target Rules

Interactive controls should have comfortable touch targets.

Avoid tiny icon buttons.

Use larger hit areas than the visible icon itself.

Do not rely on hover for important information on touch devices.

Admin table actions should move into menus on narrow screens when necessary.

---

## 58. Dark Mode Strategy

Dark mode is optional for Phase 1.

Light mode is the primary supported theme.

If dark mode is implemented:

- Use semantic tokens.
- Do not hardcode light-only colors.
- Ensure charts remain readable.
- Ensure status colors remain distinguishable.
- Test image and card surfaces.

The theme preference may be stored as UI preference.

Theme preference is not business data.

---

## 59. AI Implementation Rules

AI-assisted development must follow the project's architecture.

AI-generated code must not introduce mock business data.

AI-generated code must not create fake API functions.

AI-generated code must not hardcode product records.

AI-generated code must not invent backend endpoints.

AI should first inspect the existing API contract.

AI should preserve existing domain and application boundaries.

AI should extend existing modules rather than replace them.

### 59.1 AI Generation Order

The recommended order for AI-assisted implementation is:

```text
1. Read backend endpoint contracts.
2. Identify existing DTOs.
3. Identify missing UI-required fields.
4. Create typed frontend DTOs.
5. Create API functions.
6. Create query keys.
7. Create TanStack Query hooks.
8. Create presentational components.
9. Create pages.
10. Add loading/error/empty states.
11. Add tests.
12. Verify no mock runtime data remains.
```

### 59.2 AI Prompt Constraint

Every AI implementation prompt should contain a rule equivalent to:

```text
Use real API responses only. Do not create mock business data, fixture products, fake orders, fake users, fake KPIs, or simulated API responses. If the backend endpoint does not exist, expose a documented unavailable state or add the required backend contract instead of inventing frontend data.
```

### 59.3 AI Backend Alignment

AI-generated frontend code must follow the uploaded backend architecture.

The frontend must align with:

- ASP.NET Core APIs.
- SQL Server persistence.
- Redis-backed services where exposed.
- Existing domain modules.
- Existing route groups.
- Existing order, payment, refund, review, and inventory concepts.

When a screen requires an endpoint not currently exposed by the backend, the implementation should identify the missing endpoint explicitly.

Do not hide missing backend capabilities behind fake frontend data.

### 59.4 AI Fixed Login Constraint

During Phase 1, AI may implement the fixed demo login.

AI must isolate it behind the authentication abstraction.

AI must not spread `if username === "admin"` checks throughout the application.

Role checks should be centralized in route guards and navigation helpers.

The eventual Keycloak migration should replace the adapter rather than rewrite feature components.

---

## 60. Phase 1 Implementation Checklist

Before moving to the homepage and product experience, confirm:

- [ ] React + TypeScript project is running.
- [ ] Tailwind CSS is configured.
- [ ] shadcn/ui primitives are installed.
- [ ] Lucide icons are available.
- [ ] TanStack Query is configured.
- [ ] React Router is configured.
- [ ] React Hook Form is configured.
- [ ] Zod is configured.
- [ ] Central API client exists.
- [ ] API errors are normalized.
- [ ] Loading state components exist.
- [ ] Empty state components exist.
- [ ] Error state components exist.
- [ ] Fixed demo login exists behind an auth abstraction.
- [ ] Logout clears the demo session.
- [ ] Customer and admin route guards exist for Phase 1.
- [ ] No runtime mock business data exists.
- [ ] No mock product arrays exist.
- [ ] No fake order arrays exist.
- [ ] No fake dashboard KPI values exist.
- [ ] No frontend-generated business IDs exist.
- [ ] Global header exists.
- [ ] Search bar exists.
- [ ] Category navigation reads from the API.
- [ ] Footer exists.
- [ ] Responsive breakpoints are implemented.
- [ ] Keyboard navigation is supported.
- [ ] Focus states are visible.
- [ ] Reduced-motion behavior is respected.

---

## 61. Phase 1 Completion Criteria

Part 1 is complete when the frontend foundation is capable of supporting the marketplace experience without architectural rework.

The application should be able to:

- Render the storefront shell.
- Render real categories from the API.
- Render real product results from the API.
- Navigate through a real product search flow.
- Maintain client-side server-state caching through TanStack Query.
- Show professional loading, error, empty, and not-found states.
- Support fixed demo login for customer and admin development flows.
- Protect customer and admin routes at the UI level for Phase 1.
- Keep business data sourced from the API.
- Remain structurally ready for future Keycloak replacement.

The next implementation part should build the complete homepage, category discovery experience, product listing page, product filters, sorting controls, product grid, and product details experience using the same design system.

---


# Part 2 — Homepage, Category Discovery, Product Listing, Search, Filtering, and Product Details

## 33. Part 2 Scope

Part 2 defines the complete customer-facing product discovery experience for the marketplace storefront. It continues directly from the design system, component architecture, API rules, fixed demo-login approach, and responsive conventions defined in Part 1.

The storefront must remain API-first. Product and category business data must be loaded from the ASP.NET Core API and persisted in SQL Server. The frontend must not introduce mock products, fake categories, hardcoded prices, invented ratings, synthetic stock counts, or placeholder sales figures as runtime business data.

The relevant source specification establishes the following public marketplace capabilities:

- Active categories are loaded from the category API.
- Active products are loaded through product search and detail APIs.
- Public product queries exclude inactive products.
- Product listing supports keyword search, category filtering, pagination, and page-size selection.
- Sorting by newest, price low-to-high, and price high-to-low requires backend query support.
- Additional price, brand, condition, stock, popularity, and rating filters require corresponding backend support.
- Product cards may display only information returned by the API.
- Product details may display images, stock, quantity, description, category, reviews, and related products only when those capabilities are supported by the backend.
- Recently added products must use real API data.
- Popular products may be shown only when backed by real sales statistics.
- Featured products may be shown only when backed by an explicit backend rule or persisted flag.
- Recently viewed products may use browser history for identifiers, but the product information itself must be refreshed from the API.

This document uses those constraints as the authoritative foundation for Part 2.

---

## 34. Marketplace Home Page

### 34.1 Purpose

The homepage is the primary discovery surface for guests and signed-in customers. Its job is to move users from broad intent to useful product discovery with minimal friction.

The homepage should answer four questions immediately:

1. What can I search for?
2. What categories can I browse?
3. What products are currently available?
4. What is the next useful action I can take?

The page must feel active without inventing business activity. When the database contains no products or categories, the page must become a polished empty state rather than fabricating content.

### 34.2 Homepage Information Hierarchy

The default desktop order is:

1. Global header.
2. Category navigation strip.
3. Search-focused hero area.
4. Quick category discovery.
5. Recently added or current product discovery section.
6. Optional featured products section when backed by real data.
7. Optional popular products section when backed by real sales statistics.
8. Optional recently viewed products section when the browser has valid viewed product identifiers.
9. Service/value proposition strip using non-transactional product-independent messaging.
10. Footer.

The page must not depend on a marketing banner to make the product discovery flow useful. The search and product content should remain the primary focus.

### 34.3 Homepage Hero

The hero area is a search-led discovery surface rather than an advertising billboard.

Required elements:

- Clear marketplace heading.
- Short explanatory supporting text.
- Large search input.
- Category selector or category shortcut.
- Search submit button.
- Optional example search text that is clearly a non-functional hint.
- Optional secondary action to browse all products.

The search input must connect to the same search and autocomplete component used by the global header. The hero should use the shared component with a larger visual treatment rather than implementing an unrelated search experience.

### 34.4 Hero Search Behavior

On desktop:

- Search control is centered or visually dominant.
- Input height should be 52–60 px.
- Search button should have a minimum 44×44 px hit area.
- Category selector may appear as a secondary adjacent control.
- Autocomplete panel aligns to the full search control width.

On tablet:

- Search remains prominent.
- Category selector may move below the input if horizontal space becomes constrained.
- Autocomplete remains full-width relative to the search region.

On mobile:

- Search control occupies the majority of the first content block.
- Category selection can be represented as a drawer, bottom sheet, or horizontal chip row.
- Search results should be reachable with one tap after selecting or typing a query.

### 34.5 Hero Empty State

If the product database is empty, the hero remains fully functional as a navigation and search entry point.

The content below may display:

- A neutral empty-state illustration or icon.
- Heading such as “Products are coming soon”.
- Supporting text explaining that no products are currently available.
- Browse categories only when real categories exist.

The empty state must not render fictional product cards.

---

## 35. Homepage Category Discovery

### 35.1 Category Data Source

Category discovery must use the active category API. Category names, identifiers, hierarchy, and display state must not be duplicated as runtime business constants in the frontend.

The frontend may define visual metadata such as icon mappings only when the mapping is presentation-only and does not become the source of category truth.

Example presentation mapping:

```ts
const categoryIconMap: Record<string, LucideIcon> = {
  electronics: Monitor,
  fashion: Shirt,
  home: House,
};
```

This mapping must never create categories that do not exist in the API.

### 35.2 Category Card

A category card may include:

- Category icon or image when available.
- Category name from API.
- Optional product count only if the API returns a real count.
- Optional short description only if supported by the API.
- Hover and focus states.
- Entire card clickable area.

Do not display fabricated product counts.

### 35.3 Category Grid

Desktop:

- 6–8 cards per row depending on viewport width.
- Consistent square or near-square tiles.
- 16–24 px gap.

Tablet:

- 3–5 cards per row.
- Cards remain large enough for touch interaction.

Mobile:

- 2 cards per row for visual browsing.
- Optional horizontal scrolling row for compact discovery.
- Avoid tiny category tiles that create accidental taps.

### 35.4 Category Loading State

While categories load:

- Render 6–8 skeleton category cards.
- Match the final card geometry.
- Preserve layout height to minimize content shift.

### 35.5 Category Error State

If category loading fails:

- Keep the global header available.
- Show a contained category error block.
- Provide a retry action.
- Avoid replacing the entire homepage with a generic server error when the rest of the page can still function.

### 35.6 Category Empty State

If the API returns no active categories:

- Hide the category grid rather than showing fake categories.
- Display a lightweight empty-state message only if category discovery is a significant part of the current viewport.
- Continue displaying other product content if products are independently available.

---

## 36. Homepage Product Discovery Sections

### 36.1 Recently Added Products

Use the product search API with a supported newest-first sort once the backend exposes that capability.

The frontend must not infer “recent” based on client fetch time. The backend owns product creation timestamps and sorting.

Suggested presentation:

- Section heading: “Recently added”.
- Optional short supporting text.
- 4 product cards on large desktop.
- 3 cards on compact desktop/tablet.
- 2 cards on mobile.
- Optional “View all” action linking to `/products?sortBy=createdAt&sortDirection=desc` when the API contract supports the parameter.

The exact query parameter names must match the generated API contract and typed API layer.

### 36.2 Featured Products

A featured section is allowed only when the backend exposes a real featured flag, rule, or query.

Do not label a random product subset as featured.

When supported:

- Use a dedicated query key.
- Keep the “featured” label visually distinct from a discount badge.
- Do not imply promotion, sponsorship, or sale unless the backend supplies the corresponding business data.

### 36.3 Popular Products

Popularity must come from real sales statistics or a backend-calculated popularity metric.

The UI may display:

- “Popular now”.
- “Top sellers”.
- “Popular in this category”.

Only use wording that accurately reflects the backend metric.

Avoid hardcoded sold counts.

### 36.4 Recently Viewed Products

Recently viewed history is allowed as a browser-side convenience feature.

The preferred model is:

```text
Browser stores ordered product IDs
        ↓
Frontend reads recent IDs
        ↓
Frontend requests current product details
        ↓
API returns current product state
        ↓
UI renders only products still available to display
```

The browser must not store authoritative product name, price, inventory, rating, or status as the source of truth.

If a recently viewed product is deleted or unavailable:

- Remove it from the visible list.
- Do not show a stale price or stale stock state.

If there are no recently viewed items:

- Hide the section by default.

### 36.5 Homepage Section Rules

Every product section must support:

- Loading state.
- Error state.
- Empty state.
- Responsive layout.
- Keyboard navigation.
- Clear section heading.
- Consistent card dimensions.

A section with no data should not leave a large unexplained blank area. Use conditional rendering or an appropriately compact empty-state message.

---

## 37. Product Listing Experience

### 37.1 Primary Route

The primary listing route is:

```text
/products
```

Category listing routes may use:

```text
/categories/:categoryId
```

Both routes should reuse the same underlying listing feature components and query logic.

### 37.2 Listing Page Responsibilities

The listing page must support:

- Search keyword.
- Category filtering.
- Pagination.
- Page-size selection.
- Supported sorting.
- Optional filters added only after corresponding backend support exists.
- Product result count when returned by the API.
- Clear active filters.
- Loading state.
- Error state.
- Empty result state.
- Responsive filter controls.

### 37.3 Page Header

The listing page header should contain:

- Breadcrumbs.
- Page title.
- Optional search phrase summary.
- Optional result count.
- Sort control.
- Filter trigger on mobile.

Example information hierarchy:

```text
Home / Electronics
Electronics
1,248 products
[ Filters ]                    [ Sort: Newest ]
```

The count must come from the API.

### 37.4 Search Query Synchronization

Search state should be represented in URL query parameters whenever possible.

Example:

```text
/products?keyword=laptop&categoryId=123&page=2&pageSize=24&sortBy=price&sortDirection=asc
```

This enables:

- Shareable search URLs.
- Browser back/forward navigation.
- Deep linking.
- Refresh persistence.
- Better debugging.

The URL must not contain client-generated business state such as fabricated stock values.

### 37.5 Query Parameter Parsing

Use a typed parser and schema.

Example conceptual type:

```ts
export interface ProductSearchParams {
  keyword?: string;
  categoryId?: string;
  minPrice?: number;
  maxPrice?: number;
  inStock?: boolean;
  condition?: string;
  brand?: string;
  sortBy?: ProductSortField;
  sortDirection?: "asc" | "desc";
  page: number;
  pageSize: number;
}
```

The frontend should reject invalid values before issuing a request.

### 37.6 Pagination Defaults

Recommended defaults:

- Desktop: 24 products per page.
- Tablet: 18 products per page.
- Mobile: 12 products per page.

However, the final page-size value should be explicit and stable once selected. Do not silently change page size on viewport resize during the same session.

The frontend should support server-provided pagination metadata:

```ts
interface PaginatedResponse<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
```

### 37.7 Pagination UX

Desktop:

- Previous.
- Numbered pages around the current page.
- Ellipses for large ranges.
- Next.

Mobile:

- Previous and next controls.
- Current page indicator.
- Optional compact page selector.

Avoid rendering 100 pagination buttons on large result sets.

### 37.8 Pagination Accessibility

Pagination must:

- Use a `<nav aria-label="Pagination">` landmark.
- Mark the current page with `aria-current="page"`.
- Use accessible button labels.
- Disable unavailable previous/next controls.
- Preserve keyboard focus after navigation when practical.

### 37.9 Sorting

Supported sorting should be driven by backend capabilities.

Initial supported options from the specification:

- Newest.
- Price: low to high.
- Price: high to low.

Additional options such as name, popularity, or rating should be displayed only after backend support is implemented.

Do not send arbitrary database column names from the browser.

Use an allowlisted frontend enum:

```ts
type ProductSortField =
  | "createdAt"
  | "price"
  | "name"
  | "popularity"
  | "rating";
```

The backend must separately validate the same supported set.

---

## 38. Product Filter Experience

### 38.1 Initial Filter Set

The first implementation may expose:

- Category.
- In-stock state when supported.
- Price range when backend support exists.

The following are later enhancements:

- Condition.
- Brand.
- Additional product attributes.
- Rating threshold.
- Seller information when a seller model exists.

Do not show controls for unsupported backend filters.

### 38.2 Desktop Filters

Desktop listing layout:

```text
┌──────────────────────────────────────────────────────────────┐
│ Breadcrumbs                                                  │
│ Title                            Sort                        │
├───────────────┬──────────────────────────────────────────────┤
│ Filters       │ Product Grid                                │
│               │                                              │
│ Category      │ [Card] [Card] [Card] [Card]                 │
│ Price         │ [Card] [Card] [Card] [Card]                 │
│ Availability  │ [Card] [Card] [Card] [Card]                 │
└───────────────┴──────────────────────────────────────────────┘
```

The filter sidebar should have a stable width of approximately 240–300 px on wide screens.

### 38.3 Mobile Filters

On mobile, filters should open in a drawer or sheet.

The filter surface should include:

- Current filter values.
- Clear all.
- Apply filters.
- Close.

The user should be able to edit multiple filters before applying them to avoid repeated network requests.

### 38.4 Filter Chip Summary

Applied filters may be represented as removable chips above the result grid.

Example:

```text
[ Electronics × ] [ In stock × ] [ Under $100 × ] [ Clear all ]
```

Chip labels should be human-readable rather than exposing raw query parameter names.

### 38.5 Price Range Input

When supported:

- Use currency-aware fields.
- Validate minimum ≤ maximum.
- Prevent negative values.
- Use server-supported currency configuration.
- Never assume a currency based solely on locale.

Do not calculate or infer currency conversions in the product listing UI unless a dedicated currency service is part of the backend contract.

### 38.6 Filter Application Strategy

The preferred mobile behavior is:

```text
Open filters
→ Change draft values
→ Apply
→ Update URL
→ Fetch API results
```

The preferred desktop behavior may support immediate application for simple filters, but the implementation should remain consistent enough that users understand when network requests occur.

### 38.7 Filter Reset

Clear all should:

- Remove supported filter query parameters.
- Reset the page to 1.
- Preserve the main search keyword unless the user explicitly clears search.
- Refetch the API data.

---

## 39. Product Grid

### 39.1 Desktop Grid

Recommended responsive behavior:

- ≥ 1440 px: 4–5 columns.
- 1200–1439 px: 4 columns.
- 992–1199 px: 3 columns.
- 768–991 px: 2–3 columns.
- 480–767 px: 2 columns.
- <480 px: 1–2 columns based on card minimum width.

The grid should be implemented with CSS Grid rather than manual positioning.

Example:

```tsx
<div className="grid grid-cols-2 gap-4 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5">
  {products.map((product) => (
    <ProductCard key={product.id} product={product} />
  ))}
</div>
```

The final class strategy should use the design tokens and actual card minimum sizes established by the project.

### 39.2 Grid Density

Product discovery pages are information-dense. Avoid overly large cards that require excessive scrolling.

Prioritize:

- Product image.
- Product name.
- Price.
- Stock or availability.
- Rating summary when API-supported.
- Wishlist action.

Secondary metadata should remain visually subordinate.

### 39.3 Grid Loading State

Use a skeleton grid with the same number of approximate cards as the expected viewport layout.

Skeleton cards should include:

- Image placeholder.
- Title lines.
- Price line.
- Optional metadata line.
- Optional action placeholder.

Do not use a full-screen spinner for ordinary product query loading.

### 39.4 Query Transition Behavior

When changing sort or filters:

- Keep previous results visible where appropriate.
- Show subtle loading indicators.
- Prevent users from interpreting stale results as updated results.
- Avoid large layout jumps.

TanStack Query should be configured to keep previous page data where appropriate and to manage cancellation of obsolete requests.

---

## 40. Product Card Specification

### 40.1 Card Anatomy

```text
┌─────────────────────────────┐
│                             │
│       Product Image         │
│                             │
├─────────────────────────────┤
│ Product Name                │
│ Rating / Review Count       │
│ Price                       │
│ Availability                │
│                             │
│ [ Add to Cart ]   [ ♡ ]     │
└─────────────────────────────┘
```

### 40.2 Card Content Rules

The card may display only API-provided values.

Allowed:

- Product name.
- Product image URL.
- Price.
- Quantity or availability state.
- Rating summary when available.
- Review count when available.
- Brand when available.
- Condition when available.
- Wishlist state for the currently signed-in customer when available from the wishlist API.

Not allowed unless supported by backend data:

- Fake discount percentages.
- Fake crossed-out previous prices.
- Random star ratings.
- Fake sold counts.
- Fake shipping speed.
- Fake stock quantities.
- Fake “best seller” badges.

### 40.3 Product Card Interaction

The primary card interaction is product navigation.

The clickable area should usually be:

- Image.
- Product title.

Interactive controls such as wishlist and add-to-cart must be separate interactive elements and must not accidentally trigger card navigation.

### 40.4 Wishlist Button

For the current Phase 1 fixed-login implementation:

- If no authenticated customer is available, the button may route to the fixed login page.
- If the customer is logged in, the button calls the wishlist API.
- The button must not claim success until the server mutation succeeds.
- Failed mutations must restore or retain the correct previous state.

The long-term Keycloak migration can replace the login mechanism without changing the card interaction model.

### 40.5 Add-to-Cart Button

The button must:

- Be disabled if the product cannot currently be purchased.
- Respect server-supported stock availability.
- Submit the mutation through the cart feature API.
- Show pending state during mutation.
- Display success feedback after the API succeeds.
- Handle stock conflicts returned by the server.

Never assume that a product is still available simply because it was available when the page loaded.

### 40.6 Card Hover

Desktop hover behavior may include:

- Slight shadow increase.
- Subtle image scale, maximum approximately 1–2%.
- Border emphasis.
- Wishlist control reveal if it remains accessible by keyboard.

Do not hide essential actions only behind hover.

### 40.7 Card Focus

Keyboard focus must be clearly visible.

The focus indicator must not depend solely on color.

### 40.8 Card Image Handling

Use a fixed aspect ratio container to prevent layout shifts.

Preferred behavior:

- `object-contain` for products where the full product should be visible.
- Neutral background surface.
- Lazy loading for below-the-fold images.
- Eager loading for the primary above-the-fold product image.

Broken images must fall back to a visually consistent placeholder without suggesting that the product has no image data unless that is true.

---

## 41. Product Detail Page

### 41.1 Route

```text
/products/:productId
```

### 41.2 Page Objectives

The product detail page must allow the customer to:

- Understand what the product is.
- Understand the current price.
- Understand availability.
- Choose quantity.
- Add to cart.
- Add to wishlist.
- Read description and product information.
- Review customer feedback when available.
- Explore related products when a real relationship exists.

### 41.3 Desktop Layout

Recommended two-column layout:

```text
┌──────────────────────────────────────────────────────────────┐
│ Breadcrumbs                                                  │
├───────────────────────────┬──────────────────────────────────┤
│ Image Gallery             │ Product Title                   │
│                           │ Rating / Reviews                │
│ Main Image                │ Price                           │
│                           │ Availability                    │
│ Thumbnails                │ Quantity                        │
│                           │ [ Add to Cart ] [ Wishlist ]     │
│                           │ Shipping info when supported    │
├───────────────────────────┴──────────────────────────────────┤
│ Description / Details                                        │
├──────────────────────────────────────────────────────────────┤
│ Reviews                                                      │
├──────────────────────────────────────────────────────────────┤
│ Related Products                                             │
└──────────────────────────────────────────────────────────────┘
```

### 41.4 Mobile Layout

Recommended order:

1. Breadcrumb or back navigation.
2. Image gallery.
3. Product title.
4. Rating summary.
5. Price.
6. Availability.
7. Quantity selector.
8. Add to cart.
9. Wishlist.
10. Product information.
11. Reviews.
12. Related products.

The primary add-to-cart action may be sticky near the bottom of the viewport on mobile, provided it does not obscure system navigation or accessibility controls.

### 41.5 Breadcrumbs

Breadcrumbs should use real category relationships when available.

Example:

```text
Home / Electronics / Laptops / Product Name
```

If the API does not provide enough hierarchical category data, simplify rather than fabricate hierarchy.

### 41.6 Product Title

The product name should be the primary page heading using `<h1>`.

Avoid decorative labels above the title unless they represent actual API data.

### 41.7 Price

Price should be visually prominent.

Use a shared `<Currency>` component for formatting.

Do not embed currency symbols directly throughout the application.

Example:

```tsx
<Currency amount={product.price} currency={currencyCode} />
```

The source of currency configuration must be centralized.

### 41.8 Stock and Availability

The UI should translate API values into clear customer language.

Examples:

- In stock.
- Limited availability.
- Out of stock.
- Temporarily unavailable.

The exact wording must reflect the actual backend data model.

Never convert a raw quantity into “Only 3 left!” unless the backend explicitly allows inventory quantities to be shown publicly and the UI business rule has been approved.

### 41.9 Quantity Selector

Quantity selection must:

- Have minimum 1.
- Respect available stock when known.
- Prevent invalid numeric input.
- Support keyboard interaction.
- Provide accessible increment/decrement labels.
- Disable increment when the maximum allowed quantity is reached.

The backend remains authoritative. A stale browser quantity must be rejected gracefully by the API.

### 41.10 Add to Cart

The primary action should have the highest visual emphasis.

States:

```text
Default
→ Adding
→ Added successfully
→ Failed
→ Stock conflict
```

Success feedback may be a toast plus a cart count update.

Do not silently fail.

### 41.11 Wishlist

The wishlist action should:

- Reflect current API state.
- Support add/remove.
- Show mutation pending state.
- Prevent duplicate submissions.
- Handle login requirement through the fixed-login flow for Phase 1.

### 41.12 Product Description

Product descriptions should be rendered as backend content.

Avoid unsafe HTML injection. Prefer sanitized structured content when rich text is supported.

### 41.13 Product Attributes

If the backend later adds structured attributes, present them as a readable specification table.

Example:

| Attribute | Value |
|---|---|
| Brand | Example Brand |
| Condition | New |
| Model | ABC-123 |

Do not display empty attribute rows.

### 41.14 Product Images

The long-term product model should support persisted image metadata and secure URLs as described by the source specification.

The UI should be designed for:

- One image.
- Multiple images.
- Thumbnail navigation.
- Fullscreen lightbox.
- Keyboard navigation.
- Touch swipe on mobile.

### 41.15 Image Gallery Desktop

Recommended layout:

- Large primary image on left.
- Vertical thumbnail rail.
- Zoom/lightbox action.

### 41.16 Image Gallery Mobile

Recommended layout:

- Full-width carousel.
- Pagination indicator.
- Optional thumbnails below.
- Swipe gestures.

The gallery must remain usable without gestures through accessible buttons.

### 41.17 Gallery Loading

Use image skeleton placeholders while metadata loads.

Do not reserve an arbitrary image height. Use the actual gallery aspect ratio.

### 41.18 Gallery Empty State

When a product has no images:

- Show a neutral product image placeholder.
- Maintain the same gallery container dimensions.
- Do not invent an image.

---

## 42. Reviews on Product Details

### 42.1 Review Summary

Display review summary only when the API returns real aggregate data.

Possible fields:

- Average rating.
- Review count.
- Distribution by rating.

Do not display a rating of zero as if it means there are zero-star reviews. Distinguish between:

- No reviews.
- Reviews with average rating.

### 42.2 Review List

Each review may display:

- Rating.
- Review title if supported.
- Review body.
- Customer display name if privacy rules permit.
- Created date.
- Verified-purchase indicator only if supported by backend logic.

### 42.3 Review Pagination

Use server pagination for large review sets.

Do not load all reviews at once by default.

### 42.4 Review Empty State

Use a friendly message such as:

```text
No reviews yet
Be the first customer to share your experience.
```

The content must not imply that the product is good or bad when there is no review data.

### 42.5 Review Eligibility

The source backend specification requires review creation to be limited to delivered purchased products. The product page may therefore expose a review action only when the authenticated customer is eligible according to the backend response.

The frontend must not independently decide that a customer is eligible merely because a product is visible.

---

## 43. Related Products

### 43.1 Data Source

Related products must come from a real backend query or supported business rule.

The initial implementation may use:

- Same category.
- Explicit related-product relationships.

Only if the API supports the relevant query.

### 43.2 Related Product Presentation

Use a horizontal product rail on desktop and mobile when practical.

The rail must:

- Support keyboard navigation.
- Provide visible scroll controls when needed.
- Avoid inaccessible horizontal scrolling.

On mobile, touch scrolling is acceptable but must not be the only mechanism.

### 43.3 Related Product Empty State

If no related products exist:

- Hide the section.

Do not show random products and label them “Related”.

---

## 44. Search and Autocomplete Integration

### 44.1 Search Experience

The search system is shared across:

- Desktop header.
- Mobile header.
- Homepage hero.
- Listing pages.

All search entry points must navigate to the same product search route.

### 44.2 Autocomplete Data

The preferred initial autocomplete model is backend-driven product/category suggestions when such an endpoint exists.

Until a dedicated autocomplete endpoint exists, the frontend should not fabricate autocomplete data from a tiny hardcoded list. It may use recent local search history as a convenience feature, but actual product suggestions must still be API-derived.

### 44.3 Autocomplete States

Support:

- Idle.
- Typing.
- Loading.
- Results.
- No results.
- Error.

### 44.4 Keyboard Navigation

Use:

- Arrow Down.
- Arrow Up.
- Enter.
- Escape.

Use ARIA combobox semantics correctly.

### 44.5 Search Suggestions

Potential groups:

```text
Products
Categories
Recent searches
```

Only render a group when the relevant data exists.

### 44.6 Recent Search History

Recent searches may be stored locally as non-authoritative UI preference data.

The stored search history must not be treated as business data.

Limit:

- 5–10 recent searches.

Allow:

- Remove individual item.
- Clear all.

### 44.7 Search Submission

On submit:

1. Trim whitespace.
2. Normalize empty input.
3. Navigate to `/products` with query parameters.
4. Reset page to 1.
5. Preserve valid selected category/filter state only where appropriate.

### 44.8 Empty Search

Submitting an empty search may navigate to the general product listing page.

It must not produce a malformed API call.

---

## 45. Category Listing Experience

### 45.1 Category Route

```text
/categories/:categoryId
```

### 45.2 Category Header

Display:

- Breadcrumbs.
- Category name.
- Category description when available.
- Product count when returned by API.
- Search/filter controls.

### 45.3 Category Product Query

The category page should reuse the product search endpoint with the category identifier.

Conceptual query:

```text
GET /api/products/search?categoryId={id}&page={page}&pageSize={pageSize}
```

The actual request should use the typed API service and configured HTTP client.

### 45.4 Missing Category

If category detail lookup returns 404:

- Display a category not-found page.
- Provide a link back to categories or products.
- Do not silently display a different category.

### 45.5 Inactive Category

Inactive categories should not be visible to public users if the backend contract excludes them.

The UI must respect the server result rather than applying a client-side status assumption.

---

## 46. Homepage to Listing Navigation

Navigation should be predictable.

Examples:

```text
Homepage category click
→ /categories/:categoryId

Homepage search
→ /products?keyword=...

Recently added
→ /products?sortBy=createdAt&sortDirection=desc

Product card
→ /products/:productId
```

The exact query parameters must remain centralized in the route/query utility module.

---

## 47. Product Search Query Keys

Recommended TanStack Query keys:

```ts
export const productKeys = {
  all: ["products"] as const,
  search: (params: ProductSearchParams) => ["products", "search", params] as const,
  detail: (id: string) => ["products", "detail", id] as const,
  related: (id: string) => ["products", "related", id] as const,
};
```

Category keys:

```ts
export const categoryKeys = {
  all: ["categories"] as const,
  list: () => ["categories", "list"] as const,
  detail: (id: string) => ["categories", "detail", id] as const,
};
```

Query key objects must contain serializable values only.

### 47.1 Cache Strategy

Suggested defaults:

- Categories: relatively long stale time.
- Product search results: moderate stale time.
- Product detail: moderate stale time.
- Reviews: shorter stale time when new reviews are expected.

The exact stale times should be tuned using actual API behavior and traffic patterns.

The browser cache is not a source of truth.

---

## 48. Product Mutation Invalidation

When a cart or wishlist mutation changes a product-related UI state:

- Invalidate the relevant cart or wishlist query.
- Update product card state from server mutation response when safe.
- Avoid manually inventing authoritative values.

When admin product changes occur:

- Invalidate product list queries.
- Invalidate affected product detail queries.
- Invalidate category counts if those counts are API-driven and impacted.

Do not blindly invalidate every query after every mutation.

---

## 49. Product Listing Error Handling

### 49.1 Network Error

Show:

- Clear heading.
- Concise explanation.
- Retry button.
- Preserve the search/filter context.

### 49.2 400 Bad Request

Likely caused by invalid query parameters.

The frontend should:

- Show a validation message.
- Reset invalid parameters if appropriate.
- Avoid retry loops.

### 49.3 404 Not Found

For product detail:

- Show not-found state.

For listing:

- Usually treat an empty valid result as an empty state rather than a 404.

### 49.4 429 Rate Limited

Show:

- Retry-later guidance.
- Preserve user input.
- Avoid automatic aggressive retries.

### 49.5 500 Server Error

Show:

- Friendly message.
- Retry action.
- Trace ID when the API provides one and the UI is appropriate for showing it.

Never expose stack traces or infrastructure secrets.

---

## 50. Product Empty States

### 50.1 No Products At All

Use a full empty-state page:

```text
No products available yet
There are currently no products in the marketplace.
```

Optional actions:

- Browse categories when categories exist.
- Return home.

### 50.2 No Search Results

Use a contextual empty state:

```text
No products matched your search
Try a different keyword or remove some filters.
```

Actions:

- Clear filters.
- Edit search.
- Browse all products.

### 50.3 No Category Results

Use:

```text
No products found in this category
Try another category or clear your filters.
```

### 50.4 Out of Stock Products

A public listing may still show out-of-stock products if the backend includes them in the public product query, but the purchase actions must correctly disable when unavailable.

The final behavior must follow the actual API contract.

---

## 51. Responsive Breakpoint Rules for Product Discovery

### 51.1 Desktop ≥ 1280 px

- Full header.
- Search with category selector.
- Persistent filter sidebar.
- 4–5 product columns.
- Two-column product detail.
- Hover enhancements enabled.

### 51.2 Tablet 768–1279 px

- Compact header.
- Search remains prominent.
- Filter sidebar may collapse.
- 2–4 product columns.
- Product detail may remain two-column at the upper tablet range.

### 51.3 Mobile < 768 px

- Compact mobile header.
- Search accessible immediately.
- Filter drawer.
- 1–2 product columns.
- Single-column product detail.
- Touch-friendly controls.
- Sticky primary action where appropriate.

### 51.4 Small Mobile < 390 px

- Avoid two-column cards when minimum content width is compromised.
- Use one-column cards when necessary.
- Reduce non-essential metadata.
- Preserve full product title visibility where possible.

---

## 52. Responsive Typography for Product Discovery

Recommended scale:

```text
Homepage hero heading
Desktop: 40–56 px
Tablet: 36–44 px
Mobile: 28–34 px

Listing page heading
Desktop: 28–36 px
Tablet: 26–32 px
Mobile: 24–28 px

Product detail title
Desktop: 32–44 px
Tablet: 28–36 px
Mobile: 24–30 px

Product card title
Desktop/tablet: 15–17 px
Mobile: 14–16 px
```

Use Tailwind semantic typography classes and centralize typography decisions in the design system.

---

## 53. Responsive Product Action Rules

Desktop:

- Add-to-cart button can be full card width.
- Wishlist button can appear as a secondary icon control.

Tablet:

- Maintain minimum tap target sizes.
- Avoid hiding actions behind hover.

Mobile:

- Buttons should be easy to tap with one hand.
- Primary button should remain visually dominant.
- Icon-only buttons require accessible labels.

Minimum recommended interactive hit area:

```text
44 × 44 px
```

---

## 54. Accessibility for Product Discovery

Every product discovery surface must meet WCAG 2.2 AA goals.

Required:

- Keyboard navigation.
- Visible focus indicators.
- Semantic headings.
- Correct landmarks.
- Alt text for meaningful product images.
- Empty alt text for purely decorative imagery.
- Sufficient color contrast.
- No color-only status communication.
- Accessible filter controls.
- Accessible autocomplete.
- Accessible pagination.
- Accessible dialogs and drawers.
- Reduced-motion support.

### 54.1 Product Card Screen Reader Order

Recommended:

1. Product name.
2. Price.
3. Availability.
4. Rating summary.
5. Add to cart.
6. Wishlist.

Do not force screen readers through decorative content before essential information.

### 54.2 Image Alternative Text

Preferred:

```text
{Product Name}
```

If the image is decorative and the product name is already adjacent, the image may use empty alt text to avoid repetition.

### 54.3 Filter Drawer Accessibility

The filter drawer must:

- Trap focus while open.
- Close with Escape.
- Return focus to the trigger.
- Have an accessible dialog label.

---

## 55. Motion Rules for Product Discovery

Motion should clarify state changes rather than decorate every interaction.

Recommended durations:

```text
Micro interaction: 100–150 ms
Standard transition: 150–250 ms
Panel/drawer: 200–300 ms
Page section reveal: 250–400 ms
```

Use easing that feels natural and does not create abrupt movement.

Respect:

```css
@media (prefers-reduced-motion: reduce) {
  * {
    animation-duration: 0.01ms !important;
    animation-iteration-count: 1 !important;
    transition-duration: 0.01ms !important;
    scroll-behavior: auto !important;
  }
}
```

### 55.1 Recommended Motion

- Product card hover elevation.
- Filter drawer slide-in.
- Autocomplete panel fade/scale.
- Toast appearance.
- Add-to-cart confirmation.
- Image gallery transitions.

### 55.2 Avoid

- Constant pulsing.
- Auto-rotating product carousels without user control.
- Excessive parallax.
- Large page transitions that slow navigation.
- Motion that hides important content.

---

## 56. Homepage Component Hierarchy

Recommended structure:

```text
<HomePage>
  <StorefrontLayout>
    <GlobalHeader />
    <CategoryNav />
    <HomeHero />
      <SearchBar />
        <SearchAutocomplete />
    <CategoryDiscoverySection>
      <SectionHeader />
      <CategoryGrid>
        <CategoryCard />
      </CategoryGrid>
    </CategoryDiscoverySection>
    <ProductSection>
      <SectionHeader />
      <ProductCarousel or ProductGrid />
        <ProductCard />
    </ProductSection>
    <RecentlyViewedSection />
    <StorefrontValueStrip />
    <Footer />
  </StorefrontLayout>
</HomePage>
```

### 56.1 Separation of Responsibilities

`HomePage`:

- Composes sections.
- Does not contain raw API calls.

`HomeHero`:

- Presentation and search navigation.

`CategoryDiscoverySection`:

- Owns category data hook composition.

`ProductSection`:

- Accepts query configuration and presentation options.

`ProductCard`:

- Displays product data.
- Delegates mutations to feature hooks.

---

## 57. Product Listing Component Hierarchy

```text
<ProductListingPage>
  <StorefrontLayout>
    <GlobalHeader />
    <Breadcrumbs />
    <ListingHeader />
      <FilterTrigger />
      <SortSelect />
    <AppliedFilterChips />
    <div className="listing-layout">
      <ProductFilterSidebar />
      <ProductResults>
        <ProductGrid>
          <ProductCard />
        </ProductGrid>
        <Pagination />
      </ProductResults>
    </div>
  </StorefrontLayout>
</ProductListingPage>
```

### 57.1 Feature Hook Responsibilities

Example:

```ts
const { data, isLoading, isError, error, refetch } = useProductSearch(params);
```

The hook should:

- Build the query key.
- Call the feature API function.
- Return TanStack Query state.

The component should:

- Render the state.
- Avoid direct HTTP calls.

---

## 58. Product Detail Component Hierarchy

```text
<ProductDetailsPage>
  <StorefrontLayout>
    <Breadcrumbs />
    <ProductDetailLayout>
      <ProductGallery />
      <ProductPurchasePanel>
        <ProductTitle />
        <ProductRatingSummary />
        <ProductPrice />
        <ProductAvailability />
        <QuantitySelector />
        <AddToCartButton />
        <WishlistButton />
      </ProductPurchasePanel>
    </ProductDetailLayout>
    <ProductInformationTabs />
    <ProductReviewsSection />
    <RelatedProductsSection />
  </StorefrontLayout>
</ProductDetailsPage>
```

### 58.1 Product Information Tabs

Possible tabs:

- Description.
- Specifications.
- Shipping.
- Reviews.

Only render tabs with actual data.

On mobile, tabs may become an accordion.

---

## 59. shadcn/ui Usage for Part 2

Recommended components:

- `Button` — search, cart, wishlist, filter actions.
- `Input` — search and price fields.
- `Select` — sorting and page size.
- `DropdownMenu` — category selector and account actions.
- `Command` — advanced autocomplete.
- `Dialog` — product image lightbox.
- `Sheet` — mobile filters.
- `Drawer` — mobile category or filter interactions.
- `Popover` — compact filter controls.
- `Badge` — status and available business labels.
- `Skeleton` — loading placeholders.
- `Pagination` — page navigation.
- `Tabs` — product information sections.
- `Accordion` — mobile product information sections.
- `Tooltip` — icon-only controls where additional explanation is useful.
- `Breadcrumb` — navigation context.
- `Carousel` — product image and related-product rails when accessibility support is maintained.
- `Toast` or project-approved notification component — action feedback.

Avoid building duplicate versions of these controls without a compelling reason.

---

## 60. Tailwind Conventions for Part 2

### 60.1 Layout

Prefer:

```text
max-w-7xl
mx-auto
px-4 sm:px-6 lg:px-8
```

Use consistent page containers rather than arbitrary widths in individual pages.

### 60.2 Grid

Use responsive grid classes derived from design tokens.

Avoid hardcoding large pixel-based card widths where CSS Grid can adapt naturally.

### 60.3 Spacing

Prefer the shared spacing scale.

Use larger vertical gaps between sections and smaller gaps within cards.

### 60.4 Borders

Use subtle neutral borders for product cards and controls.

Do not rely on heavy borders to separate every element.

### 60.5 Radius

Use the design system radius consistently.

Product cards may use a medium or large radius.

Buttons and inputs should use the shared component radius.

---

## 61. API Integration Rules for Part 2

### 61.1 Categories

Public category data must be retrieved from:

```text
GET /api/categories
```

or the exact generated API contract corresponding to active categories.

### 61.2 Product Search

Use:

```text
GET /api/products/search
```

with supported query parameters.

### 61.3 Product Detail

Use:

```text
GET /api/products/{id}
```

### 61.4 Reviews

Use:

```text
GET /api/product-reviews/product/{productId}
```

### 61.5 API Client Flow

```text
UI component
    ↓
feature hook
    ↓
feature API function
    ↓
core apiClient
    ↓
ASP.NET Core API
    ↓
Application layer
    ↓
Domain / Infrastructure
    ↓
SQL Server / Redis where applicable
```

### 61.6 No Direct Component HTTP Calls

Forbidden:

```tsx
useEffect(() => {
  fetch("/api/products");
}, []);
```

Preferred:

```tsx
const { data, isLoading } = useProductSearch(params);
```

### 61.7 Server Authority

The frontend must trust server responses for:

- Price.
- Stock.
- Product status.
- Availability.
- Review summary.
- Product count.
- Pagination metadata.

---

## 62. Fixed Login Integration for Product Discovery

The current phase uses a fixed login approach.

The product discovery pages remain publicly accessible.

Customer-only actions:

- Wishlist.
- Add-to-cart if the current implementation requires login.
- Checkout.

When the visitor is not logged in and attempts a protected action:

```text
User clicks protected action
        ↓
Fixed Login Page
        ↓
Successful login
        ↓
Return to intended route/action when practical
```

### 62.1 Login Redirect State

Store only temporary navigation context, for example:

```ts
interface LoginRedirectState {
  returnTo: string;
}
```

Do not store credentials or sensitive business data in localStorage.

### 62.2 Future Keycloak Swap

The fixed login implementation should sit behind an interface such as:

```ts
interface AuthSession {
  user: CurrentUser | null;
  isAuthenticated: boolean;
  login(): Promise<void>;
  logout(): Promise<void>;
}
```

Later, the implementation can be replaced by Keycloak without changing:

- Product cards.
- Product detail pages.
- Wishlist buttons.
- Cart UI.
- Route layout structure.

---

## 63. Homepage Performance

### 63.1 Above-the-Fold Priority

Prioritize:

- Header.
- Search.
- Primary category discovery.
- First visible product section.

### 63.2 Image Loading

Use:

- Responsive image sizes where supported.
- Lazy loading below the fold.
- Explicit dimensions or aspect ratio containers.

### 63.3 Query Parallelism

Independent homepage queries may load in parallel:

```text
Categories
Recently added products
Featured products
Popular products
Recently viewed products
```

Do not block the entire homepage on one optional section.

### 63.4 Progressive Rendering

Render sections independently.

For example:

```text
Header loads
→ Search available
→ Categories render
→ Recently added section renders
→ Optional featured section renders
```

A failing optional section should not blank the entire page.

---

## 64. Listing Performance

Recommended:

- URL-based query state.
- TanStack Query caching.
- Request cancellation.
- Stable query keys.
- Paginated server queries.
- Lazy-loaded images.
- Avoid unnecessary refetching on every render.

Avoid:

- Fetching all products and filtering in the browser.
- Loading all reviews with every product result.
- Re-fetching identical queries unnecessarily.
- Rendering hundreds of DOM nodes when pagination is available.

---

## 65. Product Detail Performance

Load independently:

1. Product details.
2. Reviews.
3. Related products.

The main product detail should not be blocked by optional sections.

Example:

```text
Product API
    ↓
Render main product
    ↓
Reviews query
    ↓
Related products query
```

If reviews fail, the core product must remain usable.

---

## 66. SEO and Shareability Foundations

Even if the application is primarily an SPA, public product routes should support predictable metadata architecture.

Product detail metadata should be derived from API data where practical.

Potential metadata:

- Title.
- Description.
- Open Graph title.
- Open Graph description.
- Canonical route.

Do not expose private customer data in metadata.

---

## 67. Analytics Events for Product Discovery

Analytics instrumentation may be added later, but event definitions should be consistent.

Suggested events:

```text
search_submitted
search_suggestion_selected
category_selected
product_list_viewed
product_viewed
product_filter_applied
product_sort_changed
product_added_to_cart
product_added_to_wishlist
review_section_viewed
```

Analytics events are observational data and must not become a replacement for the transactional API.

Do not encode sensitive customer information into analytics event payloads.

---

## 68. Testing Requirements for Part 2

### 68.1 Unit Tests

Test:

- Search parameter parsing.
- Search parameter serialization.
- Filter validation.
- Sort validation.
- Currency formatting.
- Product card state rendering.
- Availability label mapping.
- Pagination calculation.

### 68.2 Component Tests

Test:

- Search bar submission.
- Autocomplete keyboard navigation.
- Category card navigation.
- Product card rendering.
- Add-to-cart pending state.
- Wishlist pending state.
- Filter drawer open/close.
- Applied filter removal.
- Pagination navigation.
- Product detail rendering.
- Image gallery controls.
- Review empty state.

### 68.3 Integration Tests

Test:

- Product search API integration.
- Category API integration.
- Product detail API integration.
- Review API integration.
- API error mapping.
- Query invalidation after mutations.

### 68.4 E2E Tests

Minimum scenarios:

1. Guest opens homepage.
2. Guest sees real categories from the API.
3. Guest searches for a real database product.
4. Guest filters by a real category.
5. Guest changes sorting when supported.
6. Guest opens a real product detail page.
7. Guest sees real stock state.
8. Customer signs in through fixed login.
9. Customer adds a real product to wishlist.
10. Customer adds a real product to cart.
11. Customer returns to product listing and sees consistent state.
12. A no-result search displays the proper empty state.
13. A missing product displays the proper not-found state.
14. A product API failure displays retry behavior.

---

## 69. AI Implementation Instructions for Part 2

When using AI coding assistants to implement Part 2, follow these rules.

### 69.1 Product Data Rule

Never ask the AI to create:

- Mock products.
- Fake categories.
- Sample ratings.
- Placeholder product prices.
- Hardcoded stock counts.
- Demo sales totals.

The AI must connect UI components to the typed API layer.

### 69.2 Component Generation Rule

When generating a product card:

1. Find the existing product TypeScript type.
2. Reuse the existing API query hook.
3. Reuse the shared Currency component.
4. Reuse the shared status/availability mapping.
5. Use shadcn/ui and Tailwind classes already established.
6. Add loading and error handling through the existing feature architecture.
7. Do not invent fields not present in the backend contract.

### 69.3 Backend Capability Rule

If a UI feature requires an API field that does not exist:

- Do not fake the field.
- Do not calculate authoritative business values in the browser.
- Mark the backend extension requirement clearly.
- Implement the UI behind a typed contract once the backend supports it.

Examples:

- Featured products require a real featured rule or field.
- Popular products require real sales statistics.
- Rating filters require a real review aggregate or query.
- Brand filtering requires brand data.
- Condition filtering requires condition data.
- Image galleries require persisted image metadata.

### 69.4 API Hook Rule

AI-generated components must follow:

```text
Component
→ Hook
→ API function
→ API client
→ Backend
```

Never:

```text
Component
→ axios.get(...)
```

### 69.5 Query State Rule

The AI must distinguish:

- `isPending` or initial loading.
- `isFetching` during background refresh.
- `isError`.
- Empty successful response.

Do not display an error state when the API returned an empty list.

### 69.6 URL State Rule

AI-generated filters and sorting should synchronize with URL query parameters.

The AI must preserve:

- Search query.
- Category.
- Page.
- Page size.
- Supported filters.
- Supported sorting.

### 69.7 Responsive Rule

Any AI-generated storefront component must be reviewed at:

- 1440 px.
- 1280 px.
- 1024 px.
- 768 px.
- 390 px.
- 320 px.

The component must not introduce horizontal page overflow.

### 69.8 Accessibility Rule

AI-generated UI must include:

- Semantic elements.
- Keyboard access.
- Focus states.
- Labels.
- ARIA only when required.
- Correct dialog behavior.
- Screen-reader-friendly status messaging.

### 69.9 Fixed Login Rule

Until Keycloak is introduced, AI-generated protected-action flows should use the fixed login abstraction.

The AI must not introduce Keycloak dependencies prematurely.

### 69.10 Future Migration Rule

The AI should keep authentication logic isolated enough that the fixed-login provider can later be replaced by Keycloak.

The AI must not couple product components directly to the fixed login implementation.

---

## 70. Part 2 Definition of Done

Part 2 is complete when:

- Homepage renders real API categories.
- Homepage renders real API products.
- No mock product or category data exists.
- Search works through the real product search API.
- Product listing uses URL-driven query state.
- Category pages reuse the listing architecture.
- Supported filters map to backend query parameters.
- Sorting is limited to backend-supported fields.
- Pagination uses server-provided metadata.
- Product cards render only API-supported information.
- Product detail pages load real product data.
- Product images use API-provided URLs or a proper empty state.
- Stock and availability come from backend data.
- Reviews load from the API.
- Related products are shown only when backed by a real backend query.
- Loading, error, empty, and not-found states exist.
- Search autocomplete is accessible.
- Filters are responsive.
- Product cards are responsive.
- The fixed login flow supports customer-only actions.
- No Keycloak integration has been introduced yet.
- Product discovery remains compatible with a future authentication-provider swap.
- Unit, component, integration, and critical E2E tests are defined.

---

# Part 2 Implementation Checklist

## Homepage

- [ ] Homepage route exists.
- [ ] Hero search is connected to the shared search component.
- [ ] Category data comes from API.
- [ ] Category loading state implemented.
- [ ] Category error state implemented.
- [ ] Category empty state implemented.
- [ ] Recently added products use API data.
- [ ] Featured products are gated by backend support.
- [ ] Popular products are gated by real sales statistics.
- [ ] Recently viewed products refresh from API.
- [ ] No fake product content exists.

## Product Listing

- [ ] `/products` route exists.
- [ ] Keyword search works.
- [ ] Category filter works.
- [ ] Pagination works.
- [ ] Page-size selection works.
- [ ] Supported sorting works.
- [ ] Unsupported sort fields cannot be sent.
- [ ] URL query state is synchronized.
- [ ] Desktop sidebar filters work.
- [ ] Mobile filter drawer works.
- [ ] Applied filter chips work.
- [ ] Clear-all works.
- [ ] Empty results state works.
- [ ] Error state works.
- [ ] Loading skeleton works.

## Product Card

- [ ] API product type is used.
- [ ] Product image renders safely.
- [ ] Product title renders.
- [ ] Price renders through Currency component.
- [ ] Availability reflects API data.
- [ ] Rating renders only when available.
- [ ] Wishlist uses real API mutation.
- [ ] Add-to-cart uses real API mutation.
- [ ] Pending mutation state works.
- [ ] Server conflict state works.
- [ ] Keyboard focus works.
- [ ] Mobile tap targets are sufficient.

## Product Details

- [ ] `/products/:productId` route exists.
- [ ] Product API query works.
- [ ] Loading state works.
- [ ] Not-found state works.
- [ ] Product gallery works.
- [ ] Product title works.
- [ ] Price works.
- [ ] Availability works.
- [ ] Quantity selector validates input.
- [ ] Add-to-cart works.
- [ ] Wishlist works.
- [ ] Description works.
- [ ] Reviews load from API.
- [ ] Review empty state works.
- [ ] Related products use real data only.
- [ ] Mobile layout works.

## Accessibility

- [ ] Search supports keyboard interaction.
- [ ] Autocomplete supports keyboard navigation.
- [ ] Filters are keyboard accessible.
- [ ] Filter drawer traps focus.
- [ ] Product cards expose meaningful labels.
- [ ] Pagination uses proper landmarks.
- [ ] Focus indicators are visible.
- [ ] Reduced-motion preferences are respected.

## AI Implementation

- [ ] AI-generated code uses feature hooks.
- [ ] AI-generated code does not use direct HTTP calls in components.
- [ ] AI-generated code does not invent backend fields.
- [ ] AI-generated code does not introduce mock data.
- [ ] AI-generated code preserves URL query state.
- [ ] AI-generated code keeps fixed login isolated.
- [ ] AI-generated code remains ready for future Keycloak migration.

---

# Part 2 Completion Marker

Part 2 establishes the storefront product-discovery layer. The next section of this document should continue with the transaction layer:

- Cart architecture and UX.
- Cart item mutations.
- Server-authoritative totals.
- Wishlist page and management.
- Checkout experience.
- Address selection and creation.
- Order creation.
- Payment page.
- Development fake payment provider UX.
- Payment success and failure states.
- Order confirmation.
- Customer order history.
- Order detail and tracking experience.
- Notifications and feedback patterns.

Authentication should remain fixed-login during the next phase until the later Keycloak migration phase is explicitly introduced.

# Part 3 — Product Details, Cart, Wishlist, Checkout, Orders, and Payment UX

## 3.0 Part Purpose

Part 3 defines the transaction and purchase journey that begins after a shopper discovers a product and continues through cart management, wishlist interactions, checkout, order creation, payment processing, and confirmation.

The implementation must preserve the core rule established by the backend specification: the browser is a presentation and interaction layer, while persisted business data remains authoritative in the ASP.NET Core API and SQL Server. The frontend may maintain temporary UI state, but it must never become the source of truth for prices, stock, order totals, payment status, refund status, or order status.

During this phase, the application uses the fixed demo login approach previously selected for the project. The user session is intentionally isolated behind an application-level authentication adapter so that a later Keycloak migration can replace the implementation without redesigning the cart, checkout, order, or payment screens.

## 3.1 Transaction Journey Overview

The primary customer journey is:

```text
Homepage
  -> Search or Category
  -> Product Listing
  -> Product Details
  -> Add to Cart
  -> Cart
  -> Checkout
  -> Address Selection
  -> Order Creation
  -> Payment
  -> Payment Result
  -> Order Confirmation
  -> Order Details
```

The alternative journey is:

```text
Product Listing
  -> Add to Wishlist
  -> Wishlist
  -> Product Details
  -> Add to Cart
  -> Checkout
```

The transaction layer must also support:

- Product stock changes between browsing and checkout.
- Price changes between browsing and checkout.
- Cart item removal.
- Quantity reduction due to current inventory.
- API validation errors.
- Network failures.
- Duplicate mutation prevention.
- Order creation conflicts.
- Payment failure.
- Payment retry when the backend supports retry.
- Empty cart protection.
- Expired or invalid checkout state.
- Browser refresh during checkout.
- Direct navigation to payment pages.
- Returning to the order after successful payment.

## 3.2 Core Transaction Rules

The frontend must follow these rules for every transaction screen:

1. Never trust client-calculated totals as authoritative.
2. Never trust a product price held in stale browser state.
3. Never assume stock is unchanged after the product detail page loaded.
4. Never mark an order as paid locally.
5. Never infer payment success from navigation alone.
6. Never generate an order ID in the frontend.
7. Never generate a payment ID in the frontend.
8. Never generate a customer ID for API ownership.
9. Never store a final order status in localStorage.
10. Never use a frontend-only cart as the permanent source of truth for authenticated customers.
11. Never bypass server validation for checkout.
12. Never let an optimistic update conceal a failed server mutation.
13. Always render authoritative values returned by the API after mutations complete.
14. Always make mutation buttons visibly pending while the request is in progress.
15. Always handle API conflicts as first-class UI states.

## 3.3 Fixed Login Integration for Phase 1

The current phase uses fixed login rather than Keycloak.

The login implementation may be a simple development-only adapter that exposes:

```ts
interface DemoAuthSession {
  isAuthenticated: boolean;
  userId: string | null;
  username: string | null;
  role: "customer" | "admin" | null;
}
```

The transaction features must not directly import the demo login implementation.

Instead, they should use an application-facing interface:

```ts
interface AuthSessionProvider {
  getSession(): DemoAuthSession;
  requireCustomer(): void;
  requireAdmin(): void;
  logout(): void;
}
```

The intent is to make the future transition from fixed login to Keycloak a provider replacement rather than a rewrite of every page.

The cart, wishlist, checkout, order, and payment features should only care whether the current application session has an authenticated customer context.

## 3.4 Product Details to Transaction Boundary

The product details page is the last discovery-oriented screen before the transaction flow begins.

The primary actions are:

- Add to cart.
- Add to wishlist.
- Quantity selection.
- Continue shopping.
- View reviews.
- View product information.

The product page should not directly create an order.

The normal sequence is:

```text
Product Details
  -> Add to Cart
  -> Cart API mutation
  -> Cart query refresh
  -> Cart confirmation feedback
```

The product page should remain usable after adding the item. It should not automatically force the shopper into checkout unless the product flow explicitly uses a dedicated "Buy Now" feature supported by the backend.

If a future direct-purchase flow is introduced, it should still create or update the server-side cart or use a dedicated backend checkout contract. It must not bypass server-side pricing and stock validation.

## 3.5 Product Detail Purchase Panel

The purchase panel should be visually prominent without overwhelming the product information.

Desktop layout:

```text
----------------------------------------------------------
| Product Gallery | Product Information | Purchase Panel |
----------------------------------------------------------
```

The purchase panel may contain:

- Current price.
- Currency.
- Stock state.
- Available quantity.
- Quantity selector.
- Add to cart button.
- Wishlist button.
- Shipping summary when supported by API data.
- Payment/support information when supported by API data.

The panel must not contain:

- Fake discounts.
- Invented shipping promises.
- Unverified delivery dates.
- Fake stock counters.
- Artificial scarcity messages.
- Random sales counts.
- Fake review counts.

## 3.6 Purchase Quantity Control

The quantity control must represent a requested quantity, not a guarantee of inventory.

Recommended UX:

```text
[-]  1  [+]
```

Rules:

- Minimum quantity is 1.
- Maximum selectable quantity may be constrained by the current API quantity when provided.
- The UI should prevent non-numeric input when possible.
- The API remains authoritative even when the UI has applied a maximum.
- If the stock changes before mutation completion, the API response wins.
- If the API reports insufficient quantity, show a conflict message and refresh the relevant product/cart data.

Example conflict message:

```text
This quantity is no longer available. The available stock has changed.
Please review the updated quantity before continuing.
```

Do not silently reduce quantity without telling the user.

## 3.7 Add-to-Cart Interaction

The Add to Cart action should use a mutation hook.

Example architecture:

```text
ProductDetailsPage
  -> useAddCartItem()
  -> cartApi.addItem()
  -> apiClient.post()
  -> ASP.NET Core API
  -> SQL Server persistence
  -> returned Cart DTO
  -> TanStack Query cache update/invalidation
```

The button states are:

```text
Default
Adding…
Added
Error
```

The "Added" state should be temporary visual feedback only. It must not replace the cart query as the source of truth.

A successful response may display:

- Inline confirmation.
- Toast notification.
- Cart item count update.
- Optional "View Cart" action.

The header cart count should be updated from the refreshed server-backed cart query.

## 3.8 Add-to-Cart Success Feedback

Recommended toast:

```text
Added to your cart
Product Name · Quantity 1
View cart
```

The toast must be accessible:

- Use the shadcn/ui toast or sonner pattern.
- Ensure screen-reader announcement.
- Avoid auto-closing so quickly that the content cannot be understood.
- Provide a visible action for "View cart" when practical.

Avoid success copy such as:

```text
Order placed!
Payment successful!
```

Those messages belong only to the appropriate transaction states.

## 3.9 Add-to-Cart Error Handling

Potential API outcomes:

| Situation | UI Behavior |
|---|---|
| Product unavailable | Show unavailable message and refresh product |
| Insufficient stock | Show stock conflict and refresh product |
| Invalid product | Show not-found state |
| Unauthorized | Redirect to fixed login |
| Rate limited | Show retry guidance |
| Server error | Show retry action |
| Network error | Show offline/retry feedback |

The UI must not hide a failed cart mutation behind a generic success toast.

## 3.10 Cart Page Purpose

The cart is the first transaction-focused page.

Primary responsibilities:

- Show current server-backed cart items.
- Allow quantity updates.
- Allow item removal.
- Show authoritative totals returned by the API.
- Show stock conflicts.
- Provide checkout entry.
- Provide continue-shopping navigation.
- Protect checkout when cart is empty.

Recommended route:

```text
/cart
```

## 3.11 Cart Page Desktop Layout

Desktop layout:

```text
-------------------------------------------------------------
| Cart                                                       |
|-----------------------------------------------------------|
| Item list                              | Order Summary     |
|----------------------------------------|-------------------|
| Product image                          | Subtotal          |
| Product name                           | Shipping          |
| Unit price                             | Tax               |
| Quantity                               | Total             |
| Remove                                 | Checkout          |
-------------------------------------------------------------
```

The item list should receive the majority of available width.

The summary column should remain visually stable while the shopper reviews items.

A sticky summary may be used on large desktop layouts, provided it does not obstruct content or create accessibility problems.

## 3.12 Cart Page Tablet Layout

Tablet layout may use:

```text
--------------------------------
| Cart                           |
|-------------------------------|
| Cart item                      |
| Cart item                      |
| Cart item                      |
|-------------------------------|
| Order Summary                 |
| Checkout                      |
--------------------------------
```

The summary moves below the item list when the viewport is too narrow for two stable columns.

## 3.13 Cart Page Mobile Layout

Mobile layout:

```text
Cart

[Product image]
Product name
Price
Quantity controls
Remove

[Product image]
Product name
Price
Quantity controls
Remove

----------------
Subtotal
Shipping
Tax
Total
[Checkout]
```

The checkout action may be sticky near the bottom of the viewport, but it must not cover content or keyboard controls.

If a sticky bottom action is used:

- Reserve safe-area space.
- Respect `env(safe-area-inset-bottom)` where relevant.
- Ensure the action remains keyboard reachable.
- Provide sufficient contrast.

## 3.14 Cart Item Component

Recommended component:

```text
CartItem
```

Props should be derived from the typed cart DTO rather than ad hoc objects.

The component should support:

- Product image.
- Product name.
- Product price.
- Quantity.
- Availability state.
- Remove action.
- Optional product link.
- Optional line total if provided by the API.

The component should never calculate the authoritative total independently.

A client-side preview can be used to avoid visual latency, but the result must be replaced by API-returned data after mutation completion.

## 3.15 Cart Quantity Mutation

The quantity update flow is:

```text
User changes quantity
  -> Disable duplicate mutation
  -> Show pending state
  -> PUT /api/shopping-carts/me/items/{productId}
  -> Receive updated cart
  -> Replace or invalidate cart query
  -> Render server values
```

When multiple quantity changes happen quickly, the UI should avoid race conditions.

Recommended strategies:

- Disable controls while the mutation is pending.
- Cancel or serialize redundant updates.
- Use the latest intended value only when the mutation layer safely guarantees order.
- Avoid applying stale mutation responses over newer data.

The backend remains authoritative.

## 3.16 Cart Remove Mutation

Removing an item should be direct but reversible where practical.

Recommended pattern:

```text
Remove
  -> mutation
  -> item disappears
  -> toast: "Item removed from your cart"
  -> optional Undo only if backend contract supports safe restoration
```

If Undo is implemented, it should call a real API mutation. It must not merely restore a local array.

Avoid confirmation dialogs for every simple remove action unless the product or cart contract indicates unusually significant consequences.

## 3.17 Clear Cart

A Clear Cart action is optional.

If implemented, it should:

- Be visually secondary.
- Require confirmation when many items would be removed.
- Call the real backend clear-cart endpoint.
- Refresh cart state after success.

Confirmation copy:

```text
Clear your cart?
All current cart items will be removed.

Cancel    Clear cart
```

## 3.18 Cart Empty State

The empty cart state should not look like an error.

Recommended content:

```text
Your cart is empty
Add products you like and they will appear here.

[Continue shopping]
```

Optional secondary action:

```text
[View wishlist]
```

Only show the wishlist action if the user is authenticated in the fixed-login system and the wishlist feature is available.

Do not inject recommended products as fake cart content.

Real recommendations can be shown only if backed by a real API response.

## 3.19 Cart Loading State

The loading state should preserve the page structure.

Use:

- Skeleton item rows.
- Skeleton product thumbnail.
- Skeleton title lines.
- Skeleton price.
- Skeleton quantity control.
- Skeleton order summary.

Avoid a full-page spinner when the cart is already loaded and a single mutation is pending.

## 3.20 Cart Error State

If the cart query fails:

```text
We couldn't load your cart
Please try again.

[Retry]
```

The page may also provide:

```text
Continue shopping
```

Do not render a fabricated empty cart when the API request failed. An error is not the same as zero items.

## 3.21 Cart Totals Contract

The cart API should return server-authoritative values where supported.

Recommended shape:

```ts
interface CartSummary {
  subtotal: number;
  shipping: number;
  tax: number;
  total: number;
  currency: string;
}
```

If the current backend does not return all of these values, the UI must not invent them.

For missing values:

- Omit the row.
- Show an explicit label such as "Calculated at checkout" only when that behavior is supported by the backend contract.

Never display `$0.00` for an unknown field merely because the frontend lacks data.

## 3.22 Cart Checkout Eligibility

The Checkout action should be enabled only when:

- The cart query succeeded.
- The cart contains at least one item.
- No blocking cart conflict exists.
- The customer has an active fixed-login session.

The UI may still allow navigation to checkout if the backend performs validation there, but it should avoid knowingly submitting an empty or invalid cart.

## 3.23 Checkout Route

Recommended route:

```text
/checkout
```

Checkout must be a dedicated, focused transaction flow.

Avoid rendering the full marketplace category navigation if it distracts from checkout completion.

Recommended layout:

```text
Checkout

1. Delivery address
2. Order review
3. Payment
4. Confirmation
```

The application may implement this as one page with sections or as a multi-step flow.

The backend specification requires the frontend to create the order through the API before processing the development payment. The UI should therefore make that sequence clear.

## 3.24 Checkout Step Model

Recommended state:

```ts
type CheckoutStep =
  | "review"
  | "address"
  | "creating-order"
  | "payment"
  | "completed";
```

The step state is UI state only.

The backend remains authoritative for:

- Order creation.
- Final prices.
- Total amounts.
- Payment state.
- Order status.

## 3.25 Checkout Information Architecture

The checkout page should include:

```text
Checkout
├── Delivery Address
├── Order Items
├── Order Summary
├── Optional Order Note
└── Place Order
```

Payment is handled after order creation according to the backend workflow:

```text
Place Order
  -> POST /api/orders
  -> payment record creation
  -> POST /api/payments/{id}/process
```

The exact API sequence should follow the implemented backend contract.

## 3.26 Checkout Address Section

Address selection should show real persisted addresses.

Recommended UI:

```text
Delivery address

(•) Home
    Name
    Street
    City, Region
    Postal code
    Country

( ) Office
    Name
    Street
    City, Region
    Postal code
    Country

[Add new address]
```

The selected address should be represented as temporary checkout selection state until the order is submitted.

The selected address ID should be submitted to the backend only if the backend contract expects it.

## 3.27 Address Card

Address cards should support:

- Radio selection.
- Edit action.
- Delete action when permitted.
- Default indicator when supported.
- Accessible label containing enough context to distinguish addresses.

Avoid exposing raw internal database IDs in visible UI.

## 3.28 Add Address During Checkout

The checkout experience should allow creating an address without forcing the user to leave the checkout flow.

Recommended pattern:

```text
[Add new address]
        |
        v
   Dialog / Sheet
        |
        v
POST /api/addresses/me
        |
        v
Refresh addresses
        |
        v
Auto-select newly created address
```

The new-address form should use the shared address schema and shared field components.

Validation errors should appear next to the relevant fields.

## 3.29 Checkout Address Form Fields

Fields should match the backend contract.

Common fields may include:

- Full name.
- Address line 1.
- Address line 2.
- City.
- State or region.
- Postal code.
- Country.
- Phone when required by the backend.

Do not add fields merely because they are common in other systems.

The implementation must follow the actual ASP.NET Core DTO and validation contract.

## 3.30 Checkout Order Review

The order review section displays:

- Product image.
- Product name.
- Quantity.
- Unit price when returned.
- Line total when returned.
- Shipping information when supported.
- Final order total.

The order review is informational.

The customer should not be able to edit prices or totals.

Quantity changes should happen in the cart before checkout unless the backend explicitly supports checkout-level quantity updates.

## 3.31 Order Note

An optional order note may be shown when the backend contract supports it.

The field should:

- Be clearly optional.
- Have a character limit matching backend validation.
- Display remaining characters when useful.
- Reject unsafe over-limit input before submission.
- Still rely on server-side validation.

If the backend does not support order notes, omit this control.

## 3.32 Checkout Summary

The checkout summary should use authoritative API values.

Layout:

```text
Order summary

Subtotal        $00.00
Shipping        $00.00
Tax             $00.00
----------------------
Total           $00.00
```

The UI must distinguish between:

- Known zero.
- Unknown.
- Not applicable.
- Calculated later.

A zero value returned by the backend should display as zero.

A missing field should not automatically display as zero.

## 3.33 Place Order Action

The primary action should be:

```text
Place order
```

The button states are:

```text
Place order
Creating order…
Order created
Error
```

Once order creation begins, the customer should not be able to accidentally submit twice.

Use:

- Disabled state.
- Progress indicator.
- Idempotency support when the backend provides it.
- Navigation only after a successful authoritative response.

## 3.34 Order Creation Flow

Recommended sequence:

```text
Checkout page
  -> Validate local form
  -> Submit order request
  -> POST /api/orders
  -> Backend recalculates total from persisted products
  -> Backend validates stock
  -> Backend persists order
  -> Frontend receives order response
  -> Redirect to payment flow
```

The frontend must not construct a final order total from client values.

The backend should calculate prices from persisted product data, as required by the project specification.

## 3.35 Order Creation Conflict

A common case is a stock change between cart load and order submission.

Example UI:

```text
We couldn't place your order

One or more items changed before checkout could be completed.
Please review your cart and try again.

[Review cart]
```

The UI should preserve useful user context where possible.

After a `409 Conflict` response:

1. Invalidate the cart query.
2. Refresh the cart.
3. Show the conflict message.
4. Keep the user on a recoverable route.

## 3.36 Checkout Authentication Guard with Fixed Login

If checkout is customer-only, the fixed-login session should be checked before submission.

Recommended behavior:

```text
Guest opens /checkout
       |
       v
Fixed Login Page
       |
       v
Return to /checkout
```

The return URL may be held in in-memory application state or a safe transient mechanism.

Do not store sensitive credentials in localStorage.

The future Keycloak implementation can replace the login adapter while preserving the route and return behavior.

## 3.37 Fixed Demo Login Page

The fixed login page should be visually simple because it is temporary infrastructure.

Recommended layout:

```text
---------------------------------------
|               Brand                 |
|                                     |
|          Sign in to continue        |
|                                     |
| Username                            |
| [______________________________]     |
|                                     |
| Password                            |
| [______________________________]     |
|                                     |
| [ Sign in ]                         |
|                                     |
| Demo accounts available             |
---------------------------------------
```

The demo credentials should be visible only in development environments.

Production builds must not ship fixed credentials.

## 3.38 Fixed Login Account Context

The fixed login may support:

```text
customer
admin
```

For this transaction layer, the customer account should provide only the minimum session context required by the existing backend contracts.

The application should still isolate the role value so that admin routing can later be replaced by real authorization.

## 3.39 Order Creation Response

The order creation response should provide enough information to transition safely to payment.

Recommended minimum data:

```ts
interface CreatedOrderSummary {
  id: string;
  total: number;
  currency: string;
  status: string;
}
```

The actual DTO must follow the backend contract.

Do not invent fields not present in the API.

## 3.40 Payment Flow Overview

The development payment flow is backend-controlled.

Required sequence from the project specification:

```text
Frontend creates order
        |
        v
Frontend creates payment
        |
        v
Backend fake payment provider processes request
        |
        v
Backend persists Pending / Paid / Failed
        |
        v
Frontend renders returned persisted state
```

The frontend must never directly set payment status to `Paid`.

## 3.41 Payment Route

Recommended routes:

```text
/payment/:paymentId
/payment/:paymentId/result
```

The first route handles the development payment action.

The second route renders the persisted result.

The UI should not assume success merely because a user clicked a button.

## 3.42 Payment Page Layout

Desktop:

```text
------------------------------------------------
| Payment                                      |
|----------------------------------------------|
| Order summary        | Payment action       |
| Order ID              | Test payment form   |
| Total                 | Test scenario      |
| Status                | Process payment    |
------------------------------------------------
```

Mobile:

```text
Payment

Order summary

Payment method

Test payment controls

[Process payment]
```

## 3.43 Development Payment UX

The payment UI must clearly indicate that it is a development/test provider.

Example banner:

```text
Development payment mode
This payment provider is for local development and testing only.
```

The payment screen may support predefined backend-supported test outcomes such as:

- Successful payment.
- Failed payment.
- Pending payment.

The frontend must send only the request shape supported by the API.

It must not send a generic `simulateSuccess: true` flag if the backend specification does not expose that contract.

## 3.44 Payment Request Rules

The payment request must be validated by the backend.

The backend must ensure:

- The order belongs to the current customer.
- The payment amount does not exceed the order total.
- The payment cannot be processed twice incorrectly.
- The payment state transition is valid.
- The order state transition is valid.

The frontend should treat any backend rejection as authoritative.

## 3.45 Payment Pending State

Pending payment should be represented explicitly.

Example:

```text
Payment pending

Your payment is still being processed.
We will show the latest payment status here.

[Refresh status]
```

Do not display a success confirmation while the persisted payment status remains pending.

## 3.46 Payment Success State

Success should be based on the backend response and persisted state.

Recommended content:

```text
Payment successful

Your order has been confirmed.

Order #ABC123
Total $00.00

[View order]
[Continue shopping]
```

The order ID must come from the API.

## 3.47 Payment Failure State

Example:

```text
Payment could not be completed

The payment provider returned a failed result.
Your order has not been marked as paid.

[Try payment again]
[View order]
```

The retry action should only appear if the backend supports a valid retry sequence.

Do not automatically create a second order for the same cart when retrying a payment.

## 3.48 Payment Result State Mapping

Use a centralized mapping:

```ts
const paymentStatusMap = {
  Pending: {
    label: "Pending",
    tone: "warning",
  },
  Paid: {
    label: "Paid",
    tone: "success",
  },
  Failed: {
    label: "Failed",
    tone: "destructive",
  },
  Refunded: {
    label: "Refunded",
    tone: "neutral",
  },
};
```

The actual status values must follow the domain enum exposed by the backend.

The UI must not define new business statuses.

## 3.49 Order Confirmation Page

Recommended route:

```text
/account/orders/:orderId
```

The confirmation state may be shown inline immediately after payment, but the canonical order page should remain available.

The confirmation page should show:

- Success or payment state.
- Order number.
- Order total.
- Order date.
- Status.
- Delivery address summary when permitted.
- Items.
- Next steps.

Avoid showing a fake countdown or fabricated delivery estimate.

## 3.50 Order Detail Information Hierarchy

Order details should prioritize:

1. Order status.
2. Order ID.
3. Payment status.
4. Total.
5. Items.
6. Delivery address.
7. Timeline.
8. Additional metadata.

The design should make the current state obvious without forcing the shopper to read every field.

## 3.51 Order Status Display

Use status badges with semantic labels.

Recommended mapping:

```text
Pending payment  -> warning
Pending           -> neutral
Confirmed         -> info
Packed            -> info
Shipped           -> info
Delivered         -> success
Cancelled         -> destructive
Payment failed    -> destructive
```

The backend may expose exact enum values that should be mapped centrally.

## 3.52 Order Timeline

When the API provides timestamps, the order detail page may display a timeline.

Example:

```text
Order placed       ●
                   |
Confirmed          ●
                   |
Packed             ●
                   |
Shipped            ●
                   |
Delivered          ●
```

The UI must not invent timestamps.

Missing timestamps should not be replaced with current time.

## 3.53 Order Timeline Accessibility

The timeline should use:

- Semantic lists.
- Text labels for status.
- Time elements when a timestamp exists.
- Icon meaning duplicated by text.

Do not rely on color alone to communicate status.

## 3.54 Order Items Section

Each order item should include:

- Product name.
- Quantity.
- Unit price where returned.
- Line total where returned.
- Product link when valid.
- Product image when supported.

Order history must render persisted order data rather than rebuilding the order from the current product catalog.

This is important because the product price or name may change after purchase.

## 3.55 Historical Order Data Rule

The order detail page should prioritize the snapshot data stored in the order aggregate.

The frontend must not replace historical values with current product values.

For example:

```text
Purchased price: $49.00
Current product price: $59.00
```

The order should display `$49.00` for the historical line item.

Current product data may be shown separately as a link to the current product page.

## 3.56 Order Address Display

The order page should show the persisted address snapshot or the backend-provided order address representation.

The UI should not always fetch the current address record and assume it represents the historical order destination.

If the API returns the current customer address instead of an order snapshot, the frontend must follow the contract rather than silently implying historical accuracy.

## 3.57 Continue Shopping from Confirmation

After successful payment, provide:

```text
[View order]
[Continue shopping]
```

The order action is primary.

The shopping action is secondary.

## 3.58 Preventing Duplicate Orders

The frontend should reduce duplicate submission risk through:

- Disabled submit button.
- Pending state.
- Single mutation lifecycle.
- Optional idempotency key if supported by backend.
- Navigation only after successful response.

The backend must still protect against duplicate creation where business requirements demand it.

A disabled button alone is not a reliable idempotency mechanism.

## 3.59 Browser Refresh During Checkout

The checkout flow should recover gracefully after refresh.

Recommended rules:

- Re-fetch cart.
- Re-fetch addresses.
- Reconstruct checkout state from server data.
- Do not trust stale client totals.
- Do not re-submit an order automatically.
- If an order already exists, allow navigation to the order detail page.

Do not store full payment credentials or sensitive payment state in browser storage.

## 3.60 Direct Navigation to Payment

If a user navigates directly to:

```text
/payment/:paymentId
```

The application should fetch payment information from the API.

It must not assume that the payment exists.

Possible outcomes:

- Valid payment.
- Payment not found.
- Unauthorized payment.
- Payment already completed.
- Payment failed.
- Payment refunded.

Each outcome should have a dedicated state.

## 3.61 Payment Already Completed

If the payment is already `Paid`, the page should not show the process button.

Instead:

```text
Payment complete

This payment has already been processed.

[View order]
```

If the payment is `Refunded`:

```text
Payment refunded

This payment has been refunded.

[View order]
```

## 3.62 Payment Authorization Errors

A user must not be allowed to view another customer's payment details.

For unauthorized access:

- `401` -> fixed login flow.
- `403` -> authorization error state.
- `404` -> not found when the API intentionally hides resource existence.

The frontend must not infer ownership from route IDs.

## 3.63 Cart and Checkout Query Keys

Recommended query keys:

```ts
export const cartKeys = {
  all: ["cart"] as const,
  me: () => ["cart", "me"] as const,
};
```

Recommended checkout keys:

```ts
export const checkoutKeys = {
  all: ["checkout"] as const,
  summary: () => ["checkout", "summary"] as const,
};
```

Checkout summary should not become a duplicate source of truth if the backend already exposes cart data.

## 3.64 Order Query Keys

Recommended:

```ts
export const orderKeys = {
  all: ["orders"] as const,
  mine: (params: OrderListParams) => ["orders", "mine", params] as const,
  detail: (id: string) => ["orders", "detail", id] as const,
};
```

The list and detail queries should be invalidated appropriately after order creation.

## 3.65 Payment Query Keys

Recommended:

```ts
export const paymentKeys = {
  all: ["payments"] as const,
  detail: (id: string) => ["payments", "detail", id] as const,
};
```

A successful payment mutation should invalidate:

- Payment detail.
- Order detail.
- Relevant order list.
- Any customer dashboard count that is directly dependent on payment state.

Only invalidate queries that are actually affected.

## 3.66 Mutation Invalidation Strategy

After adding a cart item:

```text
invalidate cart/me
```

After updating cart quantity:

```text
invalidate cart/me
```

After removing a cart item:

```text
invalidate cart/me
```

After creating an order:

```text
invalidate cart/me
invalidate orders/mine
```

After processing payment:

```text
invalidate payment/detail/:id
invalidate order/detail/:id
invalidate orders/mine
```

Do not invalidate the entire application cache indiscriminately.

## 3.67 Optimistic Updates

Optimistic updates may be used sparingly.

Good candidates:

- Wishlist toggle.
- Cart quantity change when rollback is reliable.

Higher-risk operations that should generally wait for server confirmation:

- Order creation.
- Payment processing.
- Refund requests.
- Inventory-affecting actions.

For every optimistic update:

1. Snapshot current state.
2. Apply optimistic change.
3. Execute mutation.
4. Roll back on error.
5. Reconcile with server response.

The final server response always wins.

## 3.68 Wishlist Page

The wishlist is a customer feature connected to the existing backend wishlist module.

Recommended route:

```text
/account/wishlist
```

The page should show:

- Product image.
- Product name.
- Current price.
- Availability.
- Add to cart.
- Remove from wishlist.
- Product link.

The wishlist must not display stale product details indefinitely.

Where the API returns product references rather than full product snapshots, the frontend should resolve current product data through supported API calls.

## 3.69 Wishlist Empty State

Recommended:

```text
Your wishlist is empty

Save products you want to revisit later.

[Explore products]
```

No sample items should be inserted.

## 3.70 Wishlist Add Flow

The wishlist action from a product page should:

```text
Click heart
  -> POST /api/wishlists/me/items/{productId}
  -> API persistence
  -> query update
  -> visual selected state
```

The selected state should reflect the server result after mutation.

## 3.71 Wishlist Remove Flow

Remove should:

- Call the API.
- Remove the item from the query state.
- Show brief feedback.
- Allow re-add through the same action.

Example:

```text
Removed from wishlist
```

## 3.72 Wishlist Toggle Button

Use a button, not a clickable icon-only `div`.

Required accessible label examples:

```text
Add to wishlist
Remove from wishlist
```

The visual heart icon should not be the only semantic signal.

## 3.73 Wishlist Product Unavailable

If the product is no longer available:

```text
No longer available
```

The user should be able to remove it from the wishlist.

Do not invent a replacement product.

## 3.74 Wishlist to Cart

The Add to Cart action should call the same cart mutation used by the product detail page.

Avoid maintaining duplicate cart logic in the wishlist feature.

Recommended shared hook:

```ts
useAddCartItem()
```

## 3.75 Wishlist Bulk Actions

Bulk operations are optional for this phase.

If implemented, they must use real API support.

Potential actions:

- Remove selected.
- Add selected to cart.

Bulk action controls should clearly communicate partial failure.

Example:

```text
3 items selected

[Add to cart]
[Remove]
```

If two of three items succeed, show the exact result rather than a generic success message.

## 3.76 Checkout and Wishlist Relationship

The checkout page should not depend on wishlist state.

The wishlist is a separate customer feature.

Navigation may include a link to wishlist, but checkout completion should not be interrupted by wishlist suggestions.

## 3.77 Order History Preview

After an order is created, the customer may see a lightweight confirmation preview.

The canonical data should still come from:

```text
GET /api/orders/me
GET /api/orders/me/{id}
```

Do not store an order object only in React state and assume it is durable.

## 3.78 Order History Empty State

Recommended:

```text
No orders yet

Orders you place will appear here.

[Start shopping]
```

The empty state must be used only when the API successfully returns zero orders.

## 3.79 Order History Loading

Use row or card skeletons matching the actual layout.

Each skeleton should represent:

- Order ID.
- Date.
- Status.
- Total.
- Item summary.
- View action.

Do not show fake order IDs during loading.

## 3.80 Order History Error

Recommended:

```text
We couldn't load your orders

[Retry]
```

Do not replace the error with an empty history message.

## 3.81 Order List Desktop Layout

Recommended columns:

```text
Order       Date        Status       Total       Action
```

Use the backend pagination contract.

The page should support:

- Pagination.
- Optional page size.
- Status filter only if backend supports it.
- Sort only if backend supports it.

## 3.82 Order List Mobile Layout

Each order becomes a card:

```text
Order #ABC123
Placed Jan 1, 2026
Status: Shipped
Total: $00.00

[View order]
```

Do not force wide desktop tables onto small mobile screens.

## 3.83 Order Detail Mobile Layout

Sections stack vertically:

```text
Order status
Order number
Payment status
Order summary
Items
Delivery address
Timeline
Actions
```

Primary actions remain visible without excessive scrolling.

## 3.84 Order Actions

Actions should be derived from backend-supported state and permissions.

Examples may include:

- View details.
- Cancel order when allowed.
- Request refund when allowed.
- Review delivered item.

The frontend must not invent state transitions.

## 3.85 Cancellation UX

If order cancellation is supported by the backend:

```text
Cancel order?

This action cannot be undone.

[Keep order] [Cancel order]
```

The cancellation request must call the API.

The UI should refresh the order after success.

## 3.86 Refund Entry Point

Refund functionality belongs to the later refund implementation scope, but the transaction UI should reserve a consistent location in order/payment details.

Do not expose a refund action unless the backend supports the endpoint and business rules.

## 3.87 Transaction Error Mapping

Use the centralized API error mapper.

Recommended mapping:

```text
400 -> Validation message
401 -> Fixed login flow
403 -> Access denied
404 -> Not found
409 -> Business conflict
429 -> Rate limit message
500 -> Generic server error with trace ID
```

A `409` during checkout should never be presented as a generic unknown error.

## 3.88 Rate Limit Messaging

For `429` responses, provide:

```text
Too many requests
Please wait a moment and try again.
```

The UI should not automatically retry mutations indefinitely.

## 3.89 Network Failure Messaging

Recommended:

```text
We couldn't connect to the server.
Check your connection and try again.
```

Avoid claiming that an order failed if the frontend cannot confirm the backend result.

This is especially important for payment processing.

## 3.90 Payment Network Uncertainty

If the payment request times out after submission, the frontend may not know whether the payment succeeded.

The UI should say:

```text
We couldn't confirm the payment result.

Please check your order and payment status before trying again.
```

Do not immediately create another payment blindly.

## 3.91 Refreshing Payment Status

If the API supports retrieving payment status, expose:

```text
[Refresh status]
```

The refresh action should:

- Fetch the persisted payment.
- Update payment state.
- Update related order state.
- Stop showing a pending state when the backend returns a terminal status.

## 3.92 Payment Polling

Polling is optional.

Use it only when:

- The backend status is genuinely asynchronous.
- Polling is supported by the contract.
- There is a clear terminal condition.

Polling should:

- Use a bounded interval.
- Stop on terminal states.
- Stop when the component unmounts.
- Avoid excessive network requests.

The UI must not poll indefinitely.

## 3.93 Cart Badge

The global cart badge should show a count derived from the cart query.

The count may represent:

- Number of distinct line items, or
- Total quantity.

The backend contract should determine which meaning is correct.

Do not switch between the two interpretations in different screens.

## 3.94 Cart Badge Accessibility

The cart trigger should expose a meaningful accessible name, for example:

```text
Cart, 3 items
```

The badge should be decorative when its information is already included in the accessible name.

## 3.95 Checkout Progress Indicator

A step indicator may show:

```text
1. Address
2. Review
3. Payment
4. Complete
```

Completed steps may be visually distinct.

The indicator should not imply that a step is complete until the corresponding operation has successfully completed.

## 3.96 Checkout Navigation Restrictions

Users should be able to go back to the cart before the order is created.

After order creation, navigation should prefer the order/payment flow.

Avoid browser-history traps that make it difficult to recover.

## 3.97 Order Submission Confirmation

Immediately after order creation, show a clear transition message:

```text
Order created

Your order #ABC123 is ready for payment.
```

Then continue to the payment page.

This is different from payment success.

## 3.98 Order vs Payment State Separation

The UI must treat these as separate concepts:

```text
Order status: PendingPayment
Payment status: Pending
```

A successful order creation does not necessarily mean the order is paid.

A failed payment does not necessarily mean the order record does not exist.

The UI must use the actual returned states.

## 3.99 Transaction State Matrix

| Order | Payment | UI Interpretation |
|---|---|---|
| PendingPayment | Pending | Payment required or processing |
| PendingPayment | Failed | Payment failed; order exists but is not paid |
| Confirmed | Paid | Successful purchase flow |
| Delivered | Paid | Completed fulfilled order |
| Any valid state | Refunded | Payment returned according to refund flow |

The exact allowed combinations are governed by backend domain rules.

## 3.100 Cart Recovery After Order

After successful order creation, the cart should be refreshed.

If the backend clears the cart as part of checkout:

```text
cart query -> empty
```

If the backend does not clear it automatically:

- Follow the API contract.
- Do not manually clear the cart in the frontend unless the backend confirms the correct behavior.

The frontend should not create an inconsistent local state by assuming cart clearing behavior.

## 3.101 Checkout Back Navigation

If a shopper returns from checkout to cart:

- Keep the cart query current.
- Re-render authoritative data.
- Preserve no invalid stale totals.

If the cart has changed on another device or tab, the latest API data wins.

## 3.102 Multi-Tab Considerations

The application may be opened in multiple browser tabs.

The UI should not assume one tab has exclusive control of the cart.

After checkout or significant cart mutation:

- Re-fetch the cart.
- Avoid relying on stale shared browser storage.

## 3.103 Product Stock Conflict During Checkout

If a product becomes unavailable:

```text
Some items are no longer available

Please return to your cart to review the updated quantities.

[Review cart]
```

The cart should show the affected item with a clear status.

## 3.104 Product Price Conflict During Checkout

If the backend detects a price change:

```text
The price of one or more items has changed.

We've updated your cart with the latest prices.
Please review your order before continuing.

[Review cart]
```

Never conceal the change.

## 3.105 Checkout Empty Cart Race Condition

A shopper may open checkout while the cart is later emptied elsewhere.

The checkout page must revalidate the current cart before final order creation.

If empty:

```text
Your cart is empty

Please add items before placing an order.

[Continue shopping]
```

## 3.106 Address Deletion During Checkout

If the selected address is deleted in another tab:

- The API should reject the order or return a validation error.
- The UI should refresh address data.
- The user should select another address.

Do not silently substitute an arbitrary address.

## 3.107 Cart and Checkout Components

Recommended shared components:

```text
components/common/
├── Currency.tsx
├── QuantitySelector.tsx
├── ProductThumbnail.tsx
├── OrderStatusBadge.tsx
├── PaymentStatusBadge.tsx
└── PriceSummary.tsx
```

Feature-specific components:

```text
features/cart/components/
├── CartPage.tsx
├── CartItem.tsx
├── CartList.tsx
├── CartSummary.tsx
├── EmptyCart.tsx
└── CartConflictBanner.tsx
```

```text
features/checkout/components/
├── CheckoutPage.tsx
├── CheckoutSteps.tsx
├── AddressSelector.tsx
├── AddressFormDialog.tsx
├── OrderReview.tsx
├── CheckoutSummary.tsx
└── PlaceOrderButton.tsx
```

```text
features/payments/components/
├── PaymentPage.tsx
├── PaymentSummary.tsx
├── PaymentStatusPanel.tsx
├── PaymentAction.tsx
└── PaymentResult.tsx
```

```text
features/orders/components/
├── OrderList.tsx
├── OrderListItem.tsx
├── OrderDetails.tsx
├── OrderTimeline.tsx
├── OrderItems.tsx
└── OrderStatusBadge.tsx
```

```text
features/wishlist/components/
├── WishlistPage.tsx
├── WishlistItem.tsx
├── WishlistGrid.tsx
└── EmptyWishlist.tsx
```

## 3.108 Cart Hook Architecture

Recommended hooks:

```ts
useCart()
useAddCartItem()
useUpdateCartItem()
useRemoveCartItem()
useClearCart()
```

Each hook should delegate to the feature API layer.

Components must not call the HTTP client directly.

## 3.109 Checkout Hook Architecture

Recommended hooks:

```ts
useCustomerAddresses()
useCreateAddress()
useCreateOrder()
```

The checkout feature may compose these hooks into a single workflow, but each backend operation should remain separately testable.

## 3.110 Payment Hook Architecture

Recommended:

```ts
usePayment(paymentId)
useCreatePayment()
useProcessPayment()
useRefreshPaymentStatus()
```

The processing hook should not update payment status manually.

## 3.111 Order Hook Architecture

Recommended:

```ts
useMyOrders(params)
useMyOrder(orderId)
```

Admin order queries should use separate hooks later.

## 3.112 Wishlist Hook Architecture

Recommended:

```ts
useWishlist()
useAddToWishlist()
useRemoveFromWishlist()
useClearWishlist()
```

The product detail and product card should share the same wishlist mutation logic.

## 3.113 Form Validation Strategy

Use React Hook Form and Zod where appropriate for client-side validation.

Validation responsibilities are split:

```text
Client validation
  -> immediate user feedback

Server validation
  -> authoritative business rules
```

Client validation must not replace backend validation.

## 3.114 Validation Error Mapping

When the backend returns field-level errors:

```json
{
  "errors": {
    "addressLine1": ["Address is required."],
    "postalCode": ["Postal code is invalid."]
  }
}
```

Map errors into React Hook Form when possible.

Global errors should appear near the form summary.

## 3.115 Form Submission Accessibility

When a form submission fails:

- Focus the first invalid field when practical.
- Provide an error summary for complex forms.
- Associate messages using `aria-describedby`.
- Preserve user-entered values.

Do not clear the whole form after validation failure.

## 3.116 Transaction Loading Indicators

Use the least disruptive loading indicator that communicates the state.

Examples:

- Cart loading -> skeleton.
- Add-to-cart mutation -> button spinner.
- Quantity update -> inline row spinner.
- Order creation -> full action lock with progress label.
- Payment processing -> dedicated processing state.

Avoid blocking the entire application for small local mutations.

## 3.117 Transaction Toast Guidelines

Use toasts for short-lived feedback:

- Added to cart.
- Removed from cart.
- Added to wishlist.
- Removed from wishlist.
- Address created.

Do not use toasts as the only communication for critical states such as:

- Payment failure.
- Payment uncertainty.
- Order conflicts.
- Validation failures.

Critical transaction outcomes should appear in persistent page content.

## 3.118 Success Messaging Hierarchy

Use:

```text
Inline success
```
for form operations.

Use:

```text
Toast
```
for lightweight mutations.

Use:

```text
Dedicated result state
```
for payment and order outcomes.

## 3.119 Button Copy Rules

Prefer explicit action labels:

```text
Add to cart
Proceed to checkout
Place order
Process payment
View order
Try again
Review cart
```

Avoid ambiguous:

```text
Submit
Continue
OK
Done
```

unless the context is unmistakable.

## 3.120 Transaction CTA Hierarchy

Primary CTA:

- Add to cart.
- Checkout.
- Place order.
- Process payment.

Secondary CTA:

- Continue shopping.
- View wishlist.
- Back to cart.

Destructive CTA:

- Remove.
- Clear cart.
- Cancel order.

Use shadcn/ui button variants consistently.

## 3.121 Destructive Action Safety

Actions such as Clear Cart and Cancel Order should require explicit intent when the consequence is significant.

Use a confirmation dialog only when the action is genuinely destructive.

Do not put destructive actions in visually identical styling to primary purchase actions.

## 3.122 Transaction Modals and Dialogs

Use shadcn/ui Dialog or AlertDialog depending on risk.

Use Dialog for:

- Add address.
- Edit address.
- Informational transaction details.

Use AlertDialog for:

- Clear cart.
- Cancel order.
- Other irreversible actions.

Dialogs must:

- Trap focus.
- Restore focus to the trigger.
- Support Escape where appropriate.
- Have accessible titles and descriptions.

## 3.123 Mobile Sheets

On mobile, address forms may use a Sheet instead of Dialog where appropriate.

The interaction should feel native to small screens while preserving the same validation and API behavior.

## 3.124 Checkout Responsive Design

Desktop:

```text
2-column
main content + summary
```

Tablet:

```text
stacked sections
summary below content
```

Mobile:

```text
single-column
compact sections
sticky primary action optional
```

The payment action must never be hidden below an excessively long summary without a clear path.

## 3.125 Transaction Touch Targets

Mobile interactive controls should be at least approximately 44 by 44 CSS pixels where practical.

This applies to:

- Quantity buttons.
- Remove buttons.
- Wishlist buttons.
- Checkout actions.
- Payment actions.
- Navigation controls.

## 3.126 Reduced Motion

Respect:

```css
@media (prefers-reduced-motion: reduce)
```

For checkout and payment:

- Reduce animated transitions.
- Avoid flashing success animations.
- Keep state transitions understandable without motion.

## 3.127 Transaction Animation Rules

Allowed:

- Cart drawer slide.
- Button loading spinner.
- Toast entrance.
- Success icon fade-in.
- Progress step transition.

Avoid:

- Large bouncing payment animations.
- Long checkout transition animations.
- Decorative motion during payment processing.

## 3.128 Cart Drawer Option

A cart drawer may be provided as a quick review layer.

It should not replace `/cart` as the full transaction page.

The drawer may show:

- Recently added items.
- Quantity summary.
- Total.
- View cart.
- Checkout.

All data must come from the cart API query.

## 3.129 Cart Drawer Accessibility

If used:

- Use shadcn/ui Sheet.
- Include a visible title.
- Include a close button.
- Trap focus.
- Return focus to the cart trigger.

The cart drawer should not automatically open repeatedly after every mutation if the user has dismissed it.

## 3.130 Checkout Summary Reusability

The `PriceSummary` component should be reusable between:

- Cart.
- Checkout.
- Order detail.
- Payment summary.

It should receive typed data instead of recalculating business values.

Example:

```tsx
<PriceSummary
  subtotal={summary.subtotal}
  shipping={summary.shipping}
  tax={summary.tax}
  total={summary.total}
  currency={summary.currency}
/>
```

If a field is unavailable, do not pass a fabricated zero.

## 3.131 Currency Formatting

Use a centralized `Currency` component.

The component should:

- Respect API currency codes.
- Use locale-aware formatting.
- Avoid manual string concatenation.
- Keep formatting consistent throughout the application.

Do not hardcode `$` if the backend supports multiple currencies.

## 3.132 Price Precision

The frontend should display the precision returned or expected by the API contract.

Avoid performing arithmetic in floating-point JavaScript for authoritative values.

If the backend returns decimal monetary values, the frontend should render them safely.

## 3.133 Product Price During Purchase

At Add to Cart time:

- The product detail page may show current price.
- The cart may refresh and display the persisted cart price.
- Checkout may revalidate against current product pricing.

Each stage may change if the backend permits price changes.

The final order response is authoritative.

## 3.134 Cart Item Identity

Use stable product IDs from the backend.

Do not use array indexes as React keys for cart line items.

Recommended:

```tsx
key={item.productId}
```

If the backend supports multiple variants, use the stable line-item ID exposed by the API.

## 3.135 Empty Product and Empty Cart Distinction

These are separate concepts.

Product listing empty state:

```text
No products found
```

Cart empty state:

```text
Your cart is empty
```

Never reuse one message for the other.

## 3.136 API Client Transaction Rules

The central API client should handle:

- Base URL.
- Headers.
- JSON serialization.
- Cancellation.
- Error normalization.
- Fixed-login session context during this phase.

Feature API functions should define domain-specific routes.

Example:

```ts
export async function getMyCart(signal?: AbortSignal) {
  return apiClient.get<CartDto>("/api/shopping-carts/me", { signal });
}
```

## 3.137 Request Cancellation

Use `AbortSignal` for queries where supported.

When a user navigates away from a page:

- Cancel obsolete requests where practical.
- Avoid state updates from stale requests.

TanStack Query should manage query lifecycle where possible.

## 3.138 Checkout Request Cancellation

Order creation should not be casually canceled after submission if the browser has already transmitted the request.

The UI may disable navigation during the critical mutation moment.

The backend remains responsible for transaction integrity.

## 3.139 Payment Request Cancellation

Do not cancel payment requests merely because the user navigated away if doing so could leave the payment state uncertain.

The application should allow the user to re-open the payment and check persisted status.

## 3.140 Test Scenario Matrix

Minimum customer transaction scenarios:

| Scenario | Expected Result |
|---|---|
| Add available product | Cart updates |
| Add unavailable product | Error state |
| Update quantity | Server-backed quantity update |
| Remove item | Item disappears after API success |
| Empty cart | Checkout blocked |
| Create address | Address persists and becomes selectable |
| Create order | Real order ID returned |
| Stock conflict | 409 handling and cart refresh |
| Payment success | Persisted Paid state |
| Payment failure | Persisted Failed state |
| Payment pending | Pending state shown |
| Payment timeout | Uncertain state with status check |
| Order history | Persisted order shown |
| Order detail | Historical values shown |
| Wishlist add | Persisted wishlist item |
| Wishlist remove | Persisted removal |

## 3.141 Playwright Transaction Tests

Minimum E2E flow:

```text
1. Sign in through fixed login.
2. Browse real products.
3. Open a real product.
4. Add the product to cart.
5. Open cart.
6. Update quantity.
7. Continue to checkout.
8. Select or create address.
9. Submit order.
10. Verify returned order ID.
11. Create/process development payment.
12. Verify payment result from API.
13. Open order details.
14. Verify persisted order state.
```

The test database must contain real seeded test data, not frontend runtime mocks.

## 3.142 Integration Test Requirements

Backend integration tests should verify:

- Cart ownership.
- Cart item persistence.
- Stock validation.
- Order total recalculation.
- Order creation transaction.
- Payment persistence.
- Fake payment provider integration.
- Order/payment state transitions.

## 3.143 Frontend Component Tests

Test:

- Quantity controls.
- Cart item rendering.
- Empty cart.
- Cart error state.
- Checkout validation.
- Address selection.
- Payment status rendering.
- Order status badge.
- Wishlist toggle.

Tests should mock HTTP at the boundary, not inject mock business data into components.

## 3.144 MSW Recommendation

For frontend tests, Mock Service Worker may be used to intercept network requests.

This is test infrastructure, not runtime business data.

Production application code must still call real API endpoints.

## 3.145 AI Rules for Transaction Features

Any AI-generated transaction code must:

1. Use the existing typed API client.
2. Use TanStack Query for server state.
3. Use feature API functions.
4. Never put HTTP calls directly in presentation components.
5. Never invent endpoints.
6. Never invent DTO fields.
7. Never calculate authoritative totals locally.
8. Never set payment status locally.
9. Never assume order state transitions.
10. Never add mock products or orders.
11. Never store sensitive payment details in browser storage.
12. Never bypass backend ownership rules.
13. Reuse shared status mappings.
14. Preserve fixed login isolation.
15. Keep future Keycloak migration possible.

## 3.146 AI Prompt Pattern for Cart Features

Use prompts such as:

```text
Implement the cart feature against the existing ASP.NET Core API.

Constraints:
- Do not create mock data.
- Use the existing apiClient.
- Use TanStack Query.
- Use /api/shopping-carts/me endpoints.
- Use server-returned totals as authoritative.
- Invalidate cart queries after mutations.
- Handle 401, 409, 429, and 500 explicitly.
- Keep fixed-login handling inside the existing auth adapter.
- Do not implement Keycloak yet.
```

## 3.147 AI Prompt Pattern for Checkout

```text
Implement checkout using the existing cart, address, and order APIs.

Requirements:
- Load real cart data.
- Load real customer addresses.
- Allow creation of a persisted address.
- Submit the order to the backend.
- Use the server-returned order total.
- Do not calculate the authoritative order total in the browser.
- Handle stock conflicts with 409 UI.
- Redirect to payment only after order creation succeeds.
- Do not process payment in the frontend.
- Keep fixed login as the current authentication mechanism.
```

## 3.148 AI Prompt Pattern for Payment

```text
Implement the development payment page.

Requirements:
- Use the existing payment API.
- Never mark payment Paid in frontend state.
- Call the backend process endpoint.
- Render persisted payment status from the API response.
- Support Pending, Paid, Failed, and Refunded states.
- Handle uncertain network outcomes safely.
- Never store card credentials.
- Never add a fake simulateSuccess field unless the backend contract explicitly supports it.
```

## 3.149 AI Prompt Pattern for Order Details

```text
Implement order details from GET /api/orders/me/{id}.

Requirements:
- Use server-returned historical order data.
- Do not replace purchased price with current product price.
- Render order and payment statuses separately.
- Render timestamps only when returned.
- Handle 404 and ownership failures.
- Do not invent delivery estimates.
```

## 3.150 Transaction Feature Acceptance Checklist

### Cart

- [ ] Cart loads from the real API.
- [ ] Empty cart state is distinct from error state.
- [ ] Cart quantity updates call the backend.
- [ ] Cart removal calls the backend.
- [ ] Cart totals come from authoritative API data.
- [ ] Checkout is blocked for an empty cart.
- [ ] Stock conflicts are visible.
- [ ] Price conflicts are visible.
- [ ] Mutation buttons show pending state.
- [ ] Duplicate mutations are prevented.

### Wishlist

- [ ] Wishlist loads from the real API.
- [ ] Add and remove actions persist.
- [ ] Product cards reuse shared wishlist mutations.
- [ ] Empty wishlist state exists.
- [ ] Unavailable wishlist items are clearly marked.

### Checkout

- [ ] Fixed login is required where customer ownership is required.
- [ ] Address data is loaded from the API.
- [ ] Address creation persists to the API.
- [ ] Order creation uses the backend.
- [ ] Order totals are server-authoritative.
- [ ] Validation errors map to the form.
- [ ] 409 conflicts are handled.
- [ ] Double-submit is prevented.

### Payment

- [ ] Payment is created through the API.
- [ ] Fake payment runs through the backend provider.
- [ ] Payment state is persisted.
- [ ] Frontend never marks payment Paid itself.
- [ ] Pending state exists.
- [ ] Failed state exists.
- [ ] Paid state exists.
- [ ] Refunded state can render when returned.
- [ ] Network uncertainty is handled safely.

### Orders

- [ ] Order history uses real API data.
- [ ] Order detail uses real API data.
- [ ] Historical prices are preserved.
- [ ] Order status is centralized.
- [ ] Payment status is separate from order status.
- [ ] Timeline uses real timestamps only.
- [ ] Empty, loading, and error states exist.

### Responsive UX

- [ ] Cart works on desktop.
- [ ] Cart works on tablet.
- [ ] Cart works on mobile.
- [ ] Checkout works on desktop.
- [ ] Checkout works on tablet.
- [ ] Checkout works on mobile.
- [ ] Payment works on desktop.
- [ ] Payment works on tablet.
- [ ] Payment works on mobile.
- [ ] Touch targets are accessible.

### AI Compliance

- [ ] No runtime mock business data introduced.
- [ ] No invented API endpoints.
- [ ] No invented DTO fields.
- [ ] No frontend-authoritative payment state.
- [ ] No frontend-authoritative order totals.
- [ ] No localStorage business source of truth.
- [ ] Fixed-login implementation remains replaceable.

# Part 3 Completion Marker

Part 3 completes the primary commerce transaction layer for the first implementation phase.

The next section should continue with the customer relationship layer:

- Customer account overview.
- Profile management.
- Address management outside checkout.
- Full wishlist experience.
- Customer order history.
- Order details and tracking.
- Review creation after delivery.
- Refund requests and refund history.
- Customer notifications.
- Account navigation and responsive account layouts.
- Customer dashboard empty, loading, and error states.
- Fixed-login customer experience integration.

Keycloak, OAuth/OIDC, JWT token handling, and production authorization remain deferred until the later migration phase and must not be introduced into Part 3 as the active authentication mechanism.
