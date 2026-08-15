const currencyFormatter = new Intl.NumberFormat("en-US", { style: "currency", currency: "USD" });
const numberFormatter = new Intl.NumberFormat("en-US");
const dateFormatter = new Intl.DateTimeFormat("en-US", { month: "short", day: "numeric", year: "numeric" });
const productStatuses: Record<number, string> = { 1: "Active", 2: "Inactive" };
const orderStatuses: Record<number, string> = { 1: "Pending", 2: "Confirmed", 3: "Packed", 4: "Shipped", 5: "Delivered", 6: "Cancelled", 7: "Pending payment", 8: "Payment failed" };
const paymentStatuses: Record<number, string> = { 1: "Pending", 2: "Paid", 3: "Failed", 4: "Refunded" };
const refundStatuses: Record<number, string> = { 1: "Pending", 2: "Approved", 3: "Rejected", 4: "Completed" };

export const formatCurrency = (value: number) => currencyFormatter.format(value);
export const formatNumber = (value: number) => numberFormatter.format(value);
export const formatDate = (value: string) => dateFormatter.format(new Date(value));
export const shortId = (value: string) => value.slice(0, 8).toUpperCase();

export const productStatusLabel = (value: number) => productStatuses[value] ?? `Status ${value}`;
export const orderStatusLabel = (value: number) => orderStatuses[value] ?? `Status ${value}`;
export const paymentStatusLabel = (value: number) => paymentStatuses[value] ?? `Status ${value}`;
export const refundStatusLabel = (value: number) => refundStatuses[value] ?? `Status ${value}`;
