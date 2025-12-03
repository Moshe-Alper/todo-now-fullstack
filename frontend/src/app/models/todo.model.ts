export interface Todo {
    id: string
    title: string
    isCompleted: boolean
    createdAt: number
}

export interface FilterBy {
    isCompleted?: boolean | null
}
